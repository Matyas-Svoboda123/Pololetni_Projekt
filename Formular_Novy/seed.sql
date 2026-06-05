-- ============================================================
-- seed.sql — Game Tracker
-- Naplní číselník platforem při prvním startu DB.
-- Spustí se automaticky po schema.sql (díky číslování 01_/02_).
-- ============================================================

-- INSERT ... ON CONFLICT DO NOTHING zabrání duplicitám
-- pokud by se seed.sql omylem spustil dvakrát
INSERT INTO platforms (name) VALUES
    ('PC'),
    ('PlayStation'),
    ('Xbox'),
    ('Nintendo Switch'),
    ('Mobile')
ON CONFLICT (name) DO NOTHING;
