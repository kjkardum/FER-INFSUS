# FER-INFSUS
Projekt u sklopu domaćih zadaća iz predmeta Informacijski sustavi na FER-u

## Pokretanje baze
Unutar datoteke /IzvorniKod/TIME-API nalazi se docker-compose.dev.yaml koji pomoću naredbe docker-compose up pokreće bazu

## Pokretanje backenda
Korištenjem IDE-a poput Rider može se pokrenuti. Backend kod nalazi se unutar /IzvorniKod/TIME-API

## Pokretanje frontenda
Izvorni kod frontend okruženja nalazi se unutar /IzvorniKod/TIME-client. 
Pokretanjem naredbe npm install && npm run dev pokreće se development environment
Pokretanje naredbe npm install && npm run build && npm run start pokreće se production environment

Obije komande pokreću frontend na http://localhost:3000