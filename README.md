# SAÉ S5.A.01 - Développement avancé | Astralis.

## Contributeurs
- **BARRIER Florian** : Product Owner ([GitHub](https://github.com/barriefl))
- **MAULINE Yannis** : Scrum Master ([GitHub](https://github.com/YannisUsmb))
- **TSANEV Pavel** : Développeur ([GitHub](https://github.com/Whitos))
- **BENECH Enzo** : Développeur ([GitHub](https://github.com/EnzoBenechIUT))

## Description du projet
Astralis est une application regroupant plusieurs bases de données dédiées aux corps célestes tels que les exoplanètes, étoiles, astéroïdes et bien d’autres. Elle propose une carte interactive du ciel, des articles scientifiques, un calendrier d’événements astronomiques, ainsi que des modélisations 3D de planètes et des contenus audio immersifs. Pour les utilisateurs les plus curieux, l’application offre également des fonctionnalités avancées telles que la classification de planètes et la prédiction d’impacts, assistées par intelligence artificielle.

## Technologies utilisées
- Back-end : C# .NET
- Base de données : PostgreSQL

## Structure du projet
Le projet est structuré comme suit :
- `/Astralis`
    - `/Astralis_API`
        - `/Properties`
            - `launchSettings.json`
        - `/Controllers`
        - `/Models`
            - `/DataManager`
            - `/DTO`
            - `/EntityFramework`
            - `/Mapper`
            - `/Repository`
            - `appsettings.json`
            - `Program.cs`
  
## Dépendances du projet (Packages NuGet)
- API
   - AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
   - Microsoft.EntityFrameworkCore.Design (8.0.11)
   - Microsoft.EntityFrameworkCore.Tools (8.0.11)
   - Npgsql.EntityFrameworkCore.PostgreSQL (8.0.11)
   - Swashbuckle.AspNetCore (6.6.2)

## Base de données
Le projet utilise PostreSQL pour stocker les données.
- SGBD : PostgreSQL 8
- Tables :

## Liens utiles
- **Dépôt GitHub** : ([Lien vers le dépôt GitHub](https://github.com/YannisUsmb/SAE05.A.08_Astralis))
- **Azure DevOps** : ([Lien vers Azure DevOps](https://dev.azure.com/SAE501Astralis/))

## Aide
Pour toute question ou assistance, n'hésitez pas à contacter l'un des contributeurs ci-dessus.
