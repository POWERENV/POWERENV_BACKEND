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
              AND VW_EVENTS_INFO.RESOLVED_AT = '2001-01-01 00:00:00.000000';
    END;
$$;

SELECT * FROM vw_events_info;

--#endregion

--#region WRITE

CREATE OR REPLACE PROCEDURE SP_INSERT_EVENT_LOG (
    _GLOBAL_EVENT_SEVERITY_LEVEL_ID INTEGER,
    _GLOBAL_EVENT_TITLE VARCHAR(100),
    _GLOBAL_EVENT_DESCRIPTION TEXT,
    TARGET_USERS_IDS INTEGER ARRAY
)
LANGUAGE plpgsql
AS $$
    DECLARE _NEW_EVENT_ID INTEGER;
    BEGIN
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
            NOW()
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
    END;
$$;

INSERT INTO GLOBAL_EVENT_SINGLE_OPERATION_LINK (
            GLOBAL_EVENT_ID,
            SINGLE_OPERATION_ID
        )
        VALUES (
            2,
            208
        );

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