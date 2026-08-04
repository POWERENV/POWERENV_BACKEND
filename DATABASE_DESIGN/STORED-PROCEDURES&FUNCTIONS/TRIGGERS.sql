CREATE OR REPLACE FUNCTION FN_TRIGGER_PGRID_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE
    FROM PPOOLS
    WHERE ppool_associaterd_pgrid_id = OLD.PGRID_ID;

    DELETE
    FROM PGRID_ACCESS_AUDITS
    WHERE pgrid_audit_target_pgrid_id = OLD.PGRID_ID;

    DELETE
    FROM PGRID_ACCESS_POLICIES
    WHERE access_policy_pgrid_id = OLD.PGRID_ID;

    RETURN OLD;
END;
$$;

CREATE OR REPLACE TRIGGER TG_PGRID_DELETE
    BEFORE DELETE ON PGRIDS
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PGRID_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PGRID_ACCESS_POLICY_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM GLOBAL_EVENT_ACCESS_POLICY_LINK
        WHERE pgrid_access_policy_id = OLD.ACCESS_POLICY_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PGRID_ACCESS_POLICY_DELETE
    BEFORE DELETE ON PGRID_ACCESS_POLICIES
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PGRID_ACCESS_POLICY_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PPOOL_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM PPOOLS_BATCH_OPERATIONS
        WHERE batch_operation_source_ppool_id = OLD.PPOOL_ID;

        DELETE
        FROM PNODES
        WHERE pnode_associated_ppool_id = OLD.PPOOL_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PPOOL_DELETE
    BEFORE DELETE ON PPOOLS
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PPOOL_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PNODE_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM LPARS
        WHERE lpar_associated_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM PNODES_ETH_ACCESS_POLICIES
        WHERE access_policy_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM PNODES_METRICS
        WHERE pnode_metric_target_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM PNODES_FSP_ERROR_LOGS
        WHERE error_log_source_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM PNODES_NIC_INFO
        WHERE pnode_nice_target_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM NODES_LOGIN_AUDITS
        WHERE pnode_login_audit_target_pnode_id = OLD.PNODE_ID;

        DELETE
        FROM PNODE_OPERATIONS
        WHERE operation_source_pnode_id = OLD.PNODE_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PNODE_DELETE
    BEFORE DELETE ON PNODES
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PNODE_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PNODE_FSP_ERROR_LOG_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM GLOBAL_EVENT_FSP_ERROR_LOG_LINK
        WHERE FSP_ERROR_LOG = OLD.ERROR_LOG_ID;

        DELETE
        FROM PNODES_FSP_ERROR_LOG_NORM_HARDWARE_FRU_RECORDS
        WHERE error_log_nhfru_error_log_id = OLD.ERROR_LOG_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PNODE_FSP_ERROR_LOG_DELETE
    BEFORE DELETE ON PNODES_FSP_ERROR_LOGS
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PNODE_FSP_ERROR_LOG_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PNODE_OPERATION_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM GLOBAL_EVENT_SINGLE_OPERATION_LINK
        WHERE single_operation_id = OLD.PNODE_OPERATION_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PNODE_OPERATION_DELETE
    BEFORE DELETE ON PNODE_OPERATIONS
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PNODE_OPERATION_DELETE();

--============================================================================================

CREATE OR REPLACE FUNCTION FN_TRIGGER_PPOOL_BATCH_OPERATION_DELETE()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
    BEGIN
        DELETE
        FROM GLOBAL_EVENT_BATCH_OPERATION_LINK
        WHERE batch_operation_id = OLD.BATCH_OPERATION_ID;

        DELETE
        FROM PNODE_OPERATIONS
        WHERE operation_batch_operation_id = OLD.BATCH_OPERATION_ID;

        RETURN OLD;
    END;
$$;

CREATE OR REPLACE TRIGGER TG_PPOOL_BATCH_OPERATION_DELETE
    BEFORE DELETE ON PPOOLS_BATCH_OPERATIONS
    FOR EACH ROW
    EXECUTE FUNCTION FN_TRIGGER_PPOOL_BATCH_OPERATION_DELETE();

--============================================================================================