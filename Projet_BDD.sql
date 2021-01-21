#DROP database mydb;
#DROP TABLE IF exists Client;
#DROP TABLE IF EXISTS produit;
#drop table if exists commande;
#DROP TABLE IF exists recette;
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS mydb DEFAULT CHARACTER SET utf8 ;
-- -----------------------------------------------------
-- Schema cooking
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema cooking
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS cooking DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE mydb ;

-- -----------------------------------------------------
-- Table mydb.`Recette`
-- -----------------------------------------------------
#drop table recette;
CREATE TABLE IF NOT EXISTS mydb.`Recette` (
  idRecette INT NOT NULL,
  Type VARCHAR(45) NULL,
  NomRecette VARCHAR(45) NULL,
  NbreIngredients VARCHAR(45) NULL,
  Descriptif VARCHAR(45) NULL,
  Prix double NULL,
  idcdr int NULL,
  compteur int NULL,
  PRIMARY KEY (idRecette))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table mydb.`Client`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS mydb.`Client` (
  NumeroTelephone VARCHAR(45),
  Nom VARCHAR(45) NULL,
  idCDR INT  NULL,
 solde int null,
  PRIMARY KEY (NumeroTelephone)
  )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table mydb.`Commande`
-- -----------------------------------------------------
#drop table commande;
CREATE TABLE IF NOT EXISTS mydb.`Commande` (
  idcommande VARCHAR(45) not null,
  Client_NumeroTelephone VARCHAR(45) NOT NULL,
  IdRecette VARCHAR(45) NOT NULL,
  
  Date datetime NULL,
  PRIMARY KEY (idcommande));




drop table if exists produit;
CREATE TABLE mydb.`Produit` (
  NomProduit VARCHAR(45) NOT NULL,
  Categorie VARCHAR(45) NULL,
 
  Stockactuel int NULL,
  StockMin int NULL,
  StockMax int NULL,
  PRIMARY KEY (NomProduit))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table mydb.`EstFait`
-- -----------------------------------------------------
drop table estFait;
CREATE TABLE IF NOT EXISTS mydb.`EstFait` (
  NomProduit varchar(45),
  IdRecette INT NOT NULL,
  
 
  Quantite VARCHAR(45) NULL,
  PRIMARY KEY (NomProduit, IdRecette));
 


-- -----------------------------------------------------
-- Table mydb.`Fournisseur`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS mydb.`Fournisseur` (
  NomFournisseur CHAR NOT NULL,
  NumTelFournisseur VARCHAR(45) NULL,
  Fournisseurcol VARCHAR(45) NOT NULL,
  PRIMARY KEY (NomFournisseur))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table mydb.`EstFournit`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS mydb.`EstFournit` (
  Produit_NomProduit CHAR NOT NULL,
  NomProduit CHAR NOT NULL,
  NomFournisseur VARCHAR(45) NOT NULL,
  PRIMARY KEY (Produit_NomProduit, NomProduit, NomFournisseur),
  INDEX fk_Produit_has_Fournisseur_Fournisseur1_idx (NomProduit ASC) VISIBLE,
  INDEX fk_Produit_has_Fournisseur_Produit1_idx (Produit_NomProduit ASC) VISIBLE,
  CONSTRAINT fk_Produit_has_Fournisseur_Produit1
    FOREIGN KEY (Produit_NomProduit)
    REFERENCES mydb.`Produit` (NomProduit)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_Produit_has_Fournisseur_Fournisseur1
    FOREIGN KEY (NomProduit)
    REFERENCES mydb.`Fournisseur` (NomFournisseur)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;



SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

INSERT INTO mydb.`client`(NumeroTelephone,`Nom`,`idCDR`,`solde`) VALUES ("0667851649","Paul",1,0);
INSERT INTO mydb.`client`(NumeroTelephone,`Nom`,`idCDR`,`solde`) VALUES ("0619516482","Ahmed",2,0);
INSERT INTO mydb.`client`(NumeroTelephone,`Nom`,`idCDR`,`solde`) VALUES ("0791814567","Moussa",3,0);
INSERT INTO mydb.`client`(NumeroTelephone,`Nom`,`idCDR`,`solde`) VALUES ("0665469173","Thomas",null,0);
INSERT INTO mydb.`client`(NumeroTelephone,`Nom`,`idCDR`,`solde`) VALUES ("0734351879","Nguyen",null,0);

INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (1,'Plat','Lasagnes','5','Lasagnes au saumon pour 1 à 2 personnes.',14,1);
INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (2,'Dessert','Tiramisu','7','Tiramisu à la fraise avec noix caramélisées',3,1);
INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (3,'Boisson','Mojito','3','Boisson au goût de menthe sans alcool.',6,2);
INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (4,'Plat','Tajine poulet','10','Tajine à la marocaine contenant poulet.',16,2);
INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (5,'Dessert','Salade de fruits','9','Salade de fruits frais selon la saison.',9,3);
INSERT INTO mydb.`recette`(idRecette,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`) VALUES (6,'Boisson','Smoothie','9','Smoothie vitaminé contenant plusieurs fruits.',8,3);
UPDATE recette SET compteur = 0 WHERE idRecette > 0;

INSERT INTO mydb.`estfait` (nomproduit,`idrecette`,`quantite`) values ("Viande",1,100);
INSERT INTO mydb.`estfait` (nomproduit,`idrecette`,`quantite`) values ("Sauce Tomate",2,200);
INSERT INTO mydb.`estfait` (nomproduit,`idrecette`,`quantite`) values ("Sel",2,300);
INSERT INTO mydb.`estfait` (nomproduit,`idrecette`,`quantite`) values ("Curry",3,150);

INSERT INTO mydb.`produit`(nomproduit,`categorie`,`stockactuel`,`stockmin`,`stockmax`) values ("Curry","Epices","1700","1000","8400");
INSERT INTO mydb.`produit`(nomproduit,`categorie`,`stockactuel`,`stockmin`,`stockmax`) values ("Sucre","Epices","1500","800","10000");
INSERT INTO mydb.`produit`(nomproduit,`categorie`,`stockactuel`,`stockmin`,`stockmax`) values ("Sel","Epices","1400","2900","12200");
INSERT INTO mydb.`produit`(nomproduit,`categorie`,`stockactuel`,`stockmin`,`stockmax`) values ("Poivre","Epices","1350","3400","4700");
