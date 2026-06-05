-- ============================================================
-- schema.sql — Game Tracker
-- Vytvoří 3 tabulky při prvním spuštění PostgreSQL kontejneru.
-- Soubor je automaticky spuštěn díky volumes v docker-compose.yaml.
-- ============================================================

-- Číselník platforem (jen read z aplikace, plníme v seed.sql)
CREATE TABLE IF NOT EXISTS platforms (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- Hlavní entita — hra
-- platform_id je cizí klíč na číselník platforms
CREATE TABLE IF NOT EXISTS games (
    id           SERIAL PRIMARY KEY,
    title        VARCHAR(200) NOT NULL,
    developer    VARCHAR(200),
    release_year INTEGER      CHECK (release_year BETWEEN 1950 AND 2100),
    platform_id  INTEGER      NOT NULL REFERENCES platforms(id),
    notes        TEXT
);

-- Dětská entita — herní relace (vztah 1:N na games)
-- ON DELETE CASCADE: smazání hry automaticky smaže i všechny její relace
CREATE TABLE IF NOT EXISTS game_sessions (
    id           SERIAL PRIMARY KEY,
    game_id      INTEGER        NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    session_date DATE           NOT NULL,
    hours_played NUMERIC(5, 2)  NOT NULL CHECK (hours_played > 0),
    notes        TEXT
);
