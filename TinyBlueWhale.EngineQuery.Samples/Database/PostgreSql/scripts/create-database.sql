SELECT 'CREATE DATABASE "EngineQuerySample"'
WHERE NOT EXISTS (
    SELECT FROM pg_database WHERE datname = 'EngineQuerySample'
)\gexec