CREATE TYPE USER_PROFILE_TYPE AS (
    USER_ID                  INTEGER,
    USER_FIRST_NAME          VARCHAR(50),
    USER_LAST_NAME           VARCHAR(50),
    USER_EMAIL               VARCHAR(100),
    USER_PASSWORD_HASH       VARCHAR(255),
    USER_PROFILE_PICTURE     VARCHAR(255),
    USER_SIGNUP_DATETIME     TIMESTAMP,
    USER_LAST_LOGIN_DATETIME TIMESTAMP
);

CREATE OR REPLACE FUNCTION FN_VALIDATE_LOGIN (
    _USER_EMAIL VARCHAR(100)
)
RETURNS USER_PROFILE_TYPE
LANGUAGE plpgsql
AS $$
    DECLARE MATCHED_USER_PROFILE USER_PROFILE_TYPE;
    BEGIN
        MATCHED_USER_PROFILE.user_id := -1;

        SELECT *
        INTO MATCHED_USER_PROFILE
        FROM USERS
        WHERE _USER_EMAIL = USERS.user_email
        LIMIT 1;

        IF NOT FOUND THEN
            MATCHED_USER_PROFILE.user_id := -1;
            MATCHED_USER_PROFILE.user_first_name := '';
            MATCHED_USER_PROFILE.user_last_name := '';
            MATCHED_USER_PROFILE.user_email := '';
            MATCHED_USER_PROFILE.user_password_hash := '';
            MATCHED_USER_PROFILE.user_profile_picture := '';
            MATCHED_USER_PROFILE.user_signup_datetime := NOW();
            MATCHED_USER_PROFILE.user_last_login_datetime := NOW();
        END IF;

        RETURN MATCHED_USER_PROFILE;
    END;
$$;

CREATE OR REPLACE PROCEDURE SP_CREATE_USER (
    _USER_FIRST_NAME VARCHAR(50),
    _USER_LAST_NAME VARCHAR(50),
    _USER_EMAIL VARCHAR(100),
    _USER_PASSWORD_HASH VARCHAR(255),
    OUT _rowsAffected INT
)
LANGUAGE plpgsql
AS $$
    DECLARE
    BEGIN
        INSERT INTO USERS (
            user_first_name,
            user_last_name,
            user_email,
            user_password_hash,
            user_profile_picture,
            user_signup_datetime,
            user_last_login_datetime
        )
        VALUES (
            _USER_FIRST_NAME,
            _USER_LAST_NAME,
            _USER_EMAIL,
            _USER_PASSWORD_HASH,
            '',
            NOW(),
            NOW()
        );

        GET DIAGNOSTICS _rowsAffected = ROW_COUNT;
    END;
$$;

ALTER SEQUENCE users_user_id_seq RESTART WITH 3;
SELECT * FROM USERS;