***********************************
*** Créer la première migration ***
***********************************

1. Ouvrir la console Package Manager Console dans Visual Studio (View -> Other Windows -> Package Manager Console)

2. Sélectionner le projet (dans Package Mangement Console sélectionner : "Default project: DataMonitoring.DAL")

3. Exécuter la commande :
    PM> add-migration MyFirstMigration

********************************************************
*** Modification structurelle de la base de données  ***
********************************************************

1. Modifications des objets Entity (dans le projet DataMonitoring.Model).
    1.1. Si vous ajoutez de nouveaux objets, il faut les rajouter à la main dans la class DataMonitoringDbContext.cs.
    1.2. Si des données par défaut sont nécessaires, les créer dans DBInitializer.cs (création lors de la première exécution)

2. Ouvrir la console PMC dans Visual Studio, sélectionner le projet DataMonitoring.DAL

3. Dans la console PMC exécutez la commande : 
    PM> Add-Migration Nom_de_la_migration
    Création du script de migration YYYYMMDDhhmmss_Nom_de_la_migration (dans DataMonitoring.DAL\Migrations\).

4. Appliquer la migration à la base de données, créez le schéma avec la commande : Update-Database.

********************************************************