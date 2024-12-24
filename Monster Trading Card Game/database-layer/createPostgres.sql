DROP DATABASE IF EXISTS mtcg_database;
CREATE DATABASE mtcg_database;

-- Tabellenstruktur für Tabelle "USER"
CREATE TABLE "USERS"(
  id SERIAL PRIMARY KEY,
  username VARCHAR(255) NOT NULL,
  password VARCHAR(255),
  token VARCHAR(255),
  statsid SERIAL,
  admin INT DEFAULT 0
);

-- Tabellenstruktur für Tabelle "STATS"
CREATE TABLE "STATS"(
  id SERIAL PRIMARY KEY,
  coins INT DEFAULT 20,
  wins INT DEFAULT 0,
  looses INT DEFAULT 0,
  elo INT DEFAULT 100,
  rating NUMERIC(5, 2)
);

-- Tabellenstruktur für Tabelle "CARDS"
CREATE TABLE "CARDS"(
  id SERIAL PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  damage INT,
  cardtype CHAR(1),
  elementtype VARCHAR(255) NOT NULL
);

-- Tabellenstruktur für Tabelle "STACKS"
CREATE TABLE "STACKS"(
  id SERIAL PRIMARY KEY,
  userid INT,
  cardid INT
);

-- Daten für Tabelle "USERS"
INSERT INTO "USERS" (username, password, token, admin) VALUES
('dupa', '54de7f606f2523cba8efac173fab42fb7f59d56ceff974c8fdb7342cf2cfe345', '-', 0),
('cycki', '54de7f606f2523cba8efac173fab42fb7f59d56ceff974c8fdb7342cf2cfe345', '-', 1);

-- Daten für Tabelle "USERS"
INSERT INTO "STATS" (coins, wins, looses, elo, rating) VALUES
(20, 0, 0, 100, 0.00),
(20, 0, 0, 100, 0.00);

-- Daten für Tabelle "CARDS"
INSERT INTO "CARDS" (name, damage, cardtype, elementtype) VALUES
('Dragon', 5, 'm', 'Fire'),
('Fire Elf', 3, 'm', 'Fire'),
('Goblin', 1, 'm', 'Normal'),
('Knight', 4, 'm', 'Normal'),
('Kraken', 8, 'm', 'Water'),
('Orc', 3, 'm', 'Fire'),
('Wizard', 6, 'm', 'Water'),
('Fireball', 6, 's', 'Fire'),
('Waterfall', 5, 's', 'Water'),
('Gale', 6, 's', 'Normal');

-- Daten für Tabelle "CARDS"
INSERT INTO "STACKS" (userid, cardid) VALUES
(1, 1), (1, 1), (1, 2), (1, 2), (1, 3), (1, 3), (1, 4), (1, 4), (1, 5), (1, 5), (1, 6), (1, 6), (1, 7), (1, 7), (1, 8), (1, 8), (1, 9), (1, 9), (1, 10), (1, 10),
(2, 1), (2, 2), (2, 2), (2, 2), (2, 3), (2, 3), (2, 4), (2, 4), (2, 5), (2, 5), (2, 6), (2, 6), (2, 7), (2, 7), (2, 8), (2, 8), (2, 9), (2, 9), (2, 10), (2, 10);

-- Fremdschlüssel für Tabelle "USERS"
ALTER TABLE "USERS"
  ADD CONSTRAINT fk_stats FOREIGN KEY (statsid) REFERENCES "STATS" (id) ON DELETE CASCADE;