-- usuário app já é criado via env; aqui criamos o usuário de replicação
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'debezium') THEN
    CREATE ROLE debezium WITH LOGIN PASSWORD 'dbz' REPLICATION;
  END IF;
END$$;

-- Banco principal (já criado via env), garantir privilégios
GRANT CONNECT ON DATABASE appdb TO app, debezium;

-- Tabela de exemplo
CREATE TABLE IF NOT EXISTS public.customers (
  id          BIGSERIAL PRIMARY KEY,
  name        TEXT NOT NULL,
  email       TEXT UNIQUE NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- gatilho para updated_at
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS trigger AS $$
BEGIN
  NEW.updated_at := now();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_set_updated_at ON public.customers;
CREATE TRIGGER trg_set_updated_at
BEFORE UPDATE ON public.customers
FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- Publicação para logical replication
-- (Debezium pode autocriar, mas aqui deixamos explícito)
DROP PUBLICATION IF EXISTS pub_appdb;
CREATE PUBLICATION pub_appdb FOR TABLE public.customers;

-- ===== Permissões para o Debezium =====
-- acesso ao schema e leitura das tabelas já existentes
GRANT USAGE ON SCHEMA public TO debezium;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO debezium;

-- (opcional, mas útil) acesso às SEQUENCES, caso alguma coluna SERIAL/BIGSERIAL seja lida
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO debezium;

-- privilégios padrão para FUTURAS tabelas/sequences criadas pelo usuário "app"
-- (como o entrypoint roda como POSTGRES_USER=app, isso já cobre seus próximos CREATE TABLE)
ALTER DEFAULT PRIVILEGES FOR ROLE app IN SCHEMA public
  GRANT SELECT ON TABLES TO debezium;

ALTER DEFAULT PRIVILEGES FOR ROLE app IN SCHEMA public
  GRANT USAGE, SELECT ON SEQUENCES TO debezium;