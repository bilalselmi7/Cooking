-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: mydb
-- ------------------------------------------------------
-- Server version	8.0.19

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cuisinier_has_recette`
--

DROP TABLE IF EXISTS `cuisinier_has_recette`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisinier_has_recette` (
  `Cuisinier_idCuisinier` int NOT NULL,
  `IdCuisinier` int NOT NULL,
  `IdRecette` varchar(45) NOT NULL,
  PRIMARY KEY (`Cuisinier_idCuisinier`,`IdCuisinier`,`IdRecette`),
  KEY `fk_Cuisinier_has_Recette_Recette1_idx` (`IdCuisinier`),
  KEY `fk_Cuisinier_has_Recette_Cuisinier1_idx` (`Cuisinier_idCuisinier`),
  CONSTRAINT `fk_Cuisinier_has_Recette_Cuisinier1` FOREIGN KEY (`Cuisinier_idCuisinier`) REFERENCES `cuisinier` (`idCuisinier`),
  CONSTRAINT `fk_Cuisinier_has_Recette_Recette1` FOREIGN KEY (`IdCuisinier`) REFERENCES `recette` (`idRecette`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier_has_recette`
--

LOCK TABLES `cuisinier_has_recette` WRITE;
/*!40000 ALTER TABLE `cuisinier_has_recette` DISABLE KEYS */;
/*!40000 ALTER TABLE `cuisinier_has_recette` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-05-10 23:38:29
