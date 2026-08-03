--#region READ

CREATE OR REPLACE PROCEDURE SP_GET_USER_EVENTS (
    _USER_ID INTEGER,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        OPEN result_set FOR
            SELECT GLOBAL_EVENT_ID,
                GLOBAL_EVENT_SEVERITY_NAME,
                GLOBAL_EVENT_TITLE,
                GLOBAL_EVENT_DESCRIPTION,
                GLOBAL_EVENT_TRIGGERED_AT,
                EVENT_TARGET_USERNAME,
                ACKNOWLEDGED_AT,
                RESOLVED_AT
            FROM VW_EVENTS_INFO
            WHERE USER_ID = _USER_ID
            ORDER BY GLOBAL_EVENT_TRIGGERED_AT DESC;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_GET_USER_LATEST_EVENTS (
    _USER_ID INTEGER,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        OPEN result_set FOR
            SELECT GLOBAL_EVENT_ID,
                GLOBAL_EVENT_SEVERITY_NAME,
                GLOBAL_EVENT_TITLE,
                GLOBAL_EVENT_DESCRIPTION,
                GLOBAL_EVENT_TRIGGERED_AT,
                EVENT_TARGET_USERNAME,
                ACKNOWLEDGED_AT,
                RESOLVED_AT
            FROM VW_EVENTS_INFO
            WHERE VW_EVENTS_INFO.USER_ID = _USER_ID
              AND VW_EVENTS_INFO.GLOBAL_EVENT_SEVERITY_LEVEL_ID >= 2
            ORDER BY GLOBAL_EVENT_TRIGGERED_AT DESC
            LIMIT 20;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_GET_USER_EVENT_TYPES_DISTRIBUTION (
    _USER_ID INTEGER,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        OPEN result_set FOR
            WITH TEST_CTE AS (
                SELECT *
                FROM VW_EVENTS_INFO
                WHERE VW_EVENTS_INFO.user_id = _USER_ID
            )
            SELECT COUNT(TEST_CTE.GLOBAL_EVENT_ID),
                   global_event_severity.GLOBAL_EVENT_SEVERITY_ID,
                   global_event_severity.GLOBAL_EVENT_SEVERITY_NAME
            FROM global_event_severity
            LEFT JOIN TEST_CTE ON TEST_CTE.global_event_severity_level_id = global_event_severity.global_event_severity_id
            GROUP BY GLOBAL_EVENT_SEVERITY.global_event_severity_id,
                     GLOBAL_EVENT_SEVERITY.global_event_severity_name
            ORDER BY GLOBAL_EVENT_SEVERITY.global_event_severity_id ASC;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_GET_USER_EVENT_LOGGING_CADENCE_STATS (
    _USER_ID INTEGER,
    MIN_DAY_TIMESTAMP TIMESTAMPTZ,
    _TIME_SCALE_UNIT VARCHAR,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE _DEFAULT_TIMEZONE VARCHAR;
    BEGIN
        SELECT DEFAULT_TIMEZONE
            INTO _DEFAULT_TIMEZONE
        FROM vw_default_timezone
        LIMIT 1;

        OPEN result_set FOR
            WITH RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE AS (
                SELECT DATE_TRUNC(_TIME_SCALE_UNIT, VW_EVENTS_INFO.global_event_triggered_at) AS PERIOD,
                    COUNT(global_event_id) AS EVENTS_CADENCE
                FROM VW_EVENTS_INFO
                WHERE VW_EVENTS_INFO.global_event_triggered_at >= MIN_DAY_TIMESTAMP
                  AND VW_EVENTS_INFO.user_id = _USER_ID
                GROUP BY PERIOD
                ORDER BY PERIOD
            ),
            TIMESTAMP_SEQUENCE_CTE AS (
                SELECT GENERATE_SERIES (
                   MIN(RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE.PERIOD),
                   (
                       CASE
                           WHEN EXTRACT(HOUR FROM (NOW() AT TIME ZONE _DEFAULT_TIMEZONE)) = 0
                               THEN date_add(NOW(), '1 hour')
                           ELSE NOW() AT TIME ZONE _DEFAULT_TIMEZONE
                       END
                   ),
                   ('1 ' || _TIME_SCALE_UNIT)::INTERVAL
                ) AS TIMESTAMP_SERIES
                FROM RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE
            )
            SELECT (TIMESTAMP_SEQUENCE_CTE.TIMESTAMP_SERIES)::VARCHAR AS HOURLY_INTERVAL_TIMESTAMP,
                   COALESCE(RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE.EVENTS_CADENCE, 0) AS EVENT_CADENCE
            FROM TIMESTAMP_SEQUENCE_CTE
            LEFT JOIN RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE ON RECORDED_HOURLY_TIMESTAMP_INTERVALS_CTE.PERIOD = TIMESTAMP_SEQUENCE_CTE.TIMESTAMP_SERIES;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_GET_ALL_USER_NOTIFICATIONS (
    _USER_ID INTEGER,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        OPEN result_set FOR
            SELECT VW_EVENTS_INFO.GLOBAL_EVENT_ID,
                GLOBAL_EVENT_SEVERITY_NAME,
                GLOBAL_EVENT_TITLE,
                GLOBAL_EVENT_DESCRIPTION,
                GLOBAL_EVENT_TRIGGERED_AT,
                EVENT_TARGET_USERNAME,
                ACKNOWLEDGED_AT,
                (
                    CASE
                        WHEN RESOLVED_AT = '2001-01-01 00:00:00.000000' THEN NULL
                    ELSE
                        RESOLVED_AT
                    END
                ) AS RESOLVED_AT
            FROM VW_EVENTS_INFO
                INNER JOIN NOTIFICATIONS ON VW_EVENTS_INFO.global_event_id = NOTIFICATIONS.global_event_id
            WHERE VW_EVENTS_INFO.USER_ID = _USER_ID
            ORDER BY GLOBAL_EVENT_TRIGGERED_AT DESC;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_GET_USER_NOTIFICATIONS (
    _USER_ID INTEGER,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        OPEN result_set FOR
            SELECT VW_EVENTS_INFO.GLOBAL_EVENT_ID,
                GLOBAL_EVENT_SEVERITY_NAME,
                GLOBAL_EVENT_TITLE,
                GLOBAL_EVENT_DESCRIPTION,
                GLOBAL_EVENT_TRIGGERED_AT,
                EVENT_TARGET_USERNAME,
                ACKNOWLEDGED_AT,
                RESOLVED_AT
            FROM VW_EVENTS_INFO
                INNER JOIN NOTIFICATIONS ON VW_EVENTS_INFO.global_event_id = NOTIFICATIONS.global_event_id
            WHERE VW_EVENTS_INFO.USER_ID = _USER_ID
              AND VW_EVENTS_INFO.RESOLVED_AT = '2001-01-01 00:00:00.000000'
            ORDER BY GLOBAL_EVENT_TRIGGERED_AT DESC;
    END;
$$;

SELECT * FROM vw_events_info;

--#endregion

--#region WRITE

CREATE OR REPLACE PROCEDURE SP_INSERT_EVENT_LOG (
    _GLOBAL_EVENT_SEVERITY_LEVEL_ID INTEGER,
    _GLOBAL_EVENT_TITLE VARCHAR(100),
    _GLOBAL_EVENT_DESCRIPTION TEXT,
    TARGET_USERS_IDS INTEGER ARRAY,
    INOUT result_set REFCURSOR  -- Add an INOUT parameter for the cursor
)
LANGUAGE plpgsql
AS $$
    DECLARE _NEW_EVENT_ID INTEGER;
        _CURRENT_DEFAULT_TIMEZONE VARCHAR;
    BEGIN
        SELECT DEFAULT_TIMEZONE
        INTO _CURRENT_DEFAULT_TIMEZONE
        FROM VW_DEFAULT_TIMEZONE;

        INSERT INTO GLOBAL_EVENTS (
           GLOBAL_EVENT_SEVERITY_LEVEL_ID,
           GLOBAL_EVENT_TITLE,
           GLOBAL_EVENT_DESCRIPTION,
           GLOBAL_EVENT_TRIGGERED_AT
        )
        VALUES (
            _GLOBAL_EVENT_SEVERITY_LEVEL_ID,
            _GLOBAL_EVENT_TITLE,
            _GLOBAL_EVENT_DESCRIPTION,
            NOW() AT TIME ZONE _CURRENT_DEFAULT_TIMEZONE
        )
        RETURNING GLOBAL_EVENT_ID INTO _NEW_EVENT_ID;

        FOR I IN 1..CARDINALITY(TARGET_USERS_IDS) LOOP
            INSERT INTO GLOBAL_EVENT_TARGET_USER_LINK (
               GLOBAL_EVENT_ID,
               TARGET_USER_ID
            )
            VALUES (
                _NEW_EVENT_ID,
                TARGET_USERS_IDS[I]
            );
        END LOOP;

        IF (_GLOBAL_EVENT_SEVERITY_LEVEL_ID >= 2) THEN
            INSERT INTO NOTIFICATIONS (
                global_event_id
            )
            VALUES (
                _NEW_EVENT_ID
            );
        END IF;

        OPEN result_set FOR
            SELECT _NEW_EVENT_ID;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_INSERT_EVENT_TO_SINGLE_OPERATION_LINK_LOG (
    _GLOBAL_EVENT_ID INTEGER,
    _SINGLE_OPERATION_ID INTEGER
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        INSERT INTO GLOBAL_EVENT_SINGLE_OPERATION_LINK (
            GLOBAL_EVENT_ID,
            SINGLE_OPERATION_ID
        )
        VALUES (
            _GLOBAL_EVENT_ID,
            _SINGLE_OPERATION_ID
        );
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_MARK_EVENT_AS_ACKNOWLEDGED (
    _EVENT_ID INTEGER,
    OUT _rowsAffected INT
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        UPDATE global_event_target_user_link
        SET acknowledged_at = NOW()
        WHERE global_event_target_user_link.global_event_id = _EVENT_ID;

        GET DIAGNOSTICS _rowsAffected = ROW_COUNT;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_MARK_EVENT_AS_RESOLVED (
    _NOTIFICATION_ID INTEGER,
    OUT _rowsAffected INT
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        UPDATE global_event_target_user_link
        SET resolved_at = NOW()
        WHERE global_event_target_user_link.global_event_id = _NOTIFICATION_ID;

        GET DIAGNOSTICS _rowsAffected = ROW_COUNT;
    END;
$$;

--#endregion