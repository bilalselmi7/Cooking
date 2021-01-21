using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Diagnostics;
using System.Xml;

namespace ProjetBDD
{
    class Program
    {
        /// <summary>
        /// Permet à l'utilisateur d'entrer un nombre en vérifiant qu'il s'agit uniquement d'un int
        /// </summary>
        /// <returns></returns>
        static int SaisieNombre()
        {
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Erreur, veuillez rentrer un nombre valide.");
            }
            return result;
        }

        /// <summary>
        /// Permet à l'utilisateur d'entrer un nombre en vérifiant qu'il s'agit uniquement d'un int
        /// </summary>
        /// <returns></returns>
        static double SaisieDouble()
        {
            double result;
            while (!double.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Erreur, veuillez rentrer un nombre valide.");
            }
            return result;
        }

        /// <summary>
        /// Permet d'écrire un texte en prenant en compte l'emplacement du curseur
        /// </summary>
        /// <param name="s">texte</param>
        /// <param name="x">abcisse</param>
        /// <param name="y">ordonnée</param>
        static void WriteAt(string s, int x, int y)
        {
            int origCol = Console.CursorLeft;
            int origRow = Console.CursorTop;
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Permet de déterminer le cdr dont les recettes ont été le plus vendu cette semaine
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CDRdelasemaine(MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = "SELECT r.idcdr,sum(compteur) FROM commande co, recette r WHERE r.IdRecette= co.IdRecette AND r.idcdr != 0 AND datediff(now(),date)<=7 GROUP BY r.idcdr HAVING sum(compteur) >= ALL(SELECT sum(compteur) FROM commande com, recette r2 WHERE r2.IdRecette = com.IdRecette AND r2.idcdr != 0 AND datediff(now(),date)<=7 GROUP BY r2.idcdr);";
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            
            Console.WriteLine("Le cdr de la semaine est le cdr numero --" + reader.GetString(0) + "-- pour un total de " + reader.GetString(1) + " plat(s) vendu(s).");
            reader.Close();
            command.Dispose();

        }

        /// <summary>
        /// Permet de supprimer un cuisinier
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Supprimercuisinier(MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            MySqlParameter numtel = new MySqlParameter("@numtel", MySqlDbType.String);
            command.Parameters.Add(numtel);
            Console.WriteLine("Entrez le numero du CDR à supprimer");
            numtel.Value = Console.ReadLine();

            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.Parameters.Add(numtel);
            command1.CommandText = "select idcdr from client where numerotelephone=@numtel;";
            MySqlDataReader reader = command1.ExecuteReader();
            reader.Read();

            try
            {
                reader.GetInt32(0);
            }
            catch
            {
                Console.WriteLine("Ce CDR n'existe pas;");
            }

            int idCDR = reader.GetInt32(0);


            reader.Close();
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = "select idrecette from recette where idcdr=@idcdr";
            MySqlParameter idcdr = new MySqlParameter("@idcdr", MySqlDbType.Int32);
            command2.Parameters.Add(idcdr);
            idcdr.Value = idCDR;
            List<int> idRecette = new List<int>();
            MySqlDataReader reader1 = command2.ExecuteReader();
            while (reader1.Read())
            {
                idRecette.Add(reader1.GetInt32(0));
            }
            reader1.Close();
            for (int i = 0; i < idRecette.Count; i++)
            {
                MySqlParameter idrecette = new MySqlParameter("@idrecette", MySqlDbType.Int32);

                MySqlCommand command3 = maConnexion.CreateCommand();
                command3.Parameters.Add(idrecette);
                idrecette.Value = idRecette[i];
                command3.CommandText = "delete from recette where idrecette=@idrecette;";
                try
                {
                    command3.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.ToString() + "Ce CDR n'a pas de recette à supprimer");
                }

            }

            command.CommandText = "delete from client where numerotelephone=@numtel;";
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("CDR  et ses recettes ont été supprimé!");


            }
            catch
            {
                Console.WriteLine("Erreur lors de la suppression.");
            }

        }

        /// <summary>
        /// Permet de supprimer une recette
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Supprimerrecette(MySqlConnection maConnexion) 
        {
            MySqlCommand command = maConnexion.CreateCommand();
            MySqlParameter idrecette = new MySqlParameter("@idrecette", MySqlDbType.Int32);
            command.Parameters.Add(idrecette);
            command.CommandText = "delete from recette where idrecette=@idrecette;";
            Console.WriteLine("Saisir l'id de la recette à supprimer");
            idrecette.Value = SaisieNombre();
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Recette supprimée!");
            }
            catch (MySqlException )
            {
                Console.WriteLine("Erreur lors de la suppression.");
            }

        }

        /// <summary>
        /// Permet de déterminer le cdr dont les recettes ont été le plus vendu
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CDROR(MySqlConnection maConnexion) 
        {

            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = "select distinct `client`.`idcdr` from client,recette where `client`.`idCDR`=`recette`.`idcdr`;";
            MySqlParameter idCDR = new MySqlParameter("@idcdr", MySqlDbType.VarChar);

            MySqlDataReader reader = command.ExecuteReader();
            List<string> idcdr = new List<string>();
            while (reader.Read())
            {
                idcdr.Add(reader.GetString(0));
            }
            reader.Close();
            command.Dispose();

            List<int> sommecompteur = new List<int>();
            for (int i = 0; i < idcdr.Count; i++)
            {
                //Console.WriteLine(idcdr[i]);

                idCDR.Value = idcdr[i];
                //Console.WriteLine(idCDR.Value);
                //Console.WriteLine(idcdr[idcdr.Count-1]);
                MySqlCommand command1 = maConnexion.CreateCommand();
                command1.Parameters.Add(idCDR);
                command1.CommandText = "select sum(compteur) from recette where idcdr=@idCDR;";
                MySqlDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    sommecompteur.Add(reader1.GetInt32(0));

                }
                reader1.Close();

            }



            int max = sommecompteur.Max();
            int index = sommecompteur.IndexOf(max);
            Console.WriteLine("Le CDR d'or est le cdr  --" + idcdr[index] + "--  pour un total de " + sommecompteur.Max() + " plat(s) vendu(s).");


        }

        /// <summary>
        /// Permet de consulter le solde de cook
        /// </summary>
        /// <param name="maConnexion"></param>
        static void ConsulterCook(MySqlConnection maConnexion) 
        {
            MySqlParameter idcdr = new MySqlParameter("@idcdr", MySqlDbType.Int32);
            Console.WriteLine("Indiquez votre idCDR pour connaitre votre solde de cook");
            //idcdr.Value = Convert.ToInt32(Console.ReadLine());
            idcdr.Value = SaisieNombre();
            MySqlCommand command = maConnexion.CreateCommand();
            command.Parameters.Add(idcdr);
            command.CommandText = "select solde from client where idcdr=@idcdr";

            MySqlDataReader reader = command.ExecuteReader();
            try
            {
                reader.Read();
                Console.WriteLine("Votre solde est de " + reader.GetString(0) + " cook(s).");
            }
            catch(MySqlException)
            {
                Console.WriteLine("Aucun id cdr ne correspond");
            }
            reader.Close();
            command.Dispose();
        } 

        /// <summary>
        /// Permet de sélectionner un produit
        /// </summary>
        /// <param name="maconnexion"></param>
        static void Selectionproduit(MySqlConnection maconnexion) 
        {
            string requete = "select nomproduit,quantite from estfait where idrecette=493;";
            MySqlCommand command = maconnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            List<string> nomproduit = new List<string>();
            List<string> qte = new List<string>();
            while (reader.Read())
            {
                nomproduit.Add(reader.GetString(0));
                qte.Add(reader.GetString(1));
            }
            reader.Close();
            command.Dispose();
            for (int i = 0; i < nomproduit.Count; i++)
            {
                Console.WriteLine(nomproduit[i] + " " + qte[i]);

            }
            string requete1 = "update  produit set stockactuel=stockactuel-@quantite where nomproduit=@nomproduit;";
            MySqlParameter quantite = new MySqlParameter("@quantite", MySqlDbType.Int32);
            MySqlParameter nomprod = new MySqlParameter("@nomproduit", MySqlDbType.VarChar);
            MySqlCommand command1 = maconnexion.CreateCommand();
            command1.CommandText = requete1;
            command1.Parameters.Add(quantite);
            command1.Parameters.Add(nomprod);
            for (int i = 0; i < nomproduit.Count; i++)
            {
                quantite.Value = Convert.ToInt32(qte[i]);
                nomprod.Value = nomproduit[i];
                try
                {
                    command1.ExecuteNonQuery();
                    Console.WriteLine("Stock MAJ ! ");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }



        } 

        /// <summary>
        /// Permet de se connecter en tant que client
        /// </summary>
        /// <param name="maConnexion"></param>
        static void ConnexionClient(MySqlConnection maConnexion)

        {
            MySqlParameter numtel = new MySqlParameter("@numtel", MySqlDbType.VarChar);
            Console.WriteLine("Numero de téléphone : ");
            numtel.Value = Console.ReadLine();
            string requete = "select * from `mydb`.`client` where `client`.`NumeroTelephone`=@numtel; ";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numtel);
            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                reader.GetString(0);
                Console.WriteLine("Vous êtes connecté .");
                reader.Close();
                command.Dispose();
            }
            catch (MySqlException)
            {
                Console.WriteLine("Erreur, votre id n'existe pas, veuillez vous inscrire");
                Console.WriteLine("APPUYEZ SUR UNE TOUCHE POUR QUITTER L'APPLICATION ");
                Console.ReadKey(true);
                int compteur = 0;
                while (compteur < 3)
                {
                    int temps = 3 - compteur;
                    Console.Write(temps + "...");
                    compteur++;
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            }
        } 

        /// <summary>
        /// Permet d'afficher toutes les recettes de la database
        /// </summary>
        /// <param name="maConnexion"></param>
        static void AfficherRecette(MySqlConnection maConnexion)
        {
            string requete = "SELECT * from `mydb`.`recette`;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            string[] valuestring = new string[reader.FieldCount];
            /*while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valuestring[i] = reader.GetValue(i).ToString();
                    Console.Write(valuestring[i] + " ");
                    if (i % 9 == 0)
                    {
                        Console.Write("\n");
                    }

                }

            }*/
            while (reader.Read())
            {
                string currentRowAsString = "";

                for (int i = 0; i < reader.FieldCount; i++)
                {

                    if (i == 0)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "IdRecette : " + valueAsString + " \n";

                    }
                    if (i == 1)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Type de la recette : " + valueAsString + " \n";

                    }
                    if (i == 2)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Nom de la recette : " + valueAsString + " \n";
                    }
                    if (i == 3)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Nombre d'ingrédients nécessaires pour cette recette : " + valueAsString + " \n";
                    }
                    if (i == 4)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Descriptif de la recette : " + valueAsString + " \n";
                    }
                    if (i == 5)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Prix de la recette : " + valueAsString + " \n";
                    }
                    if (i == 6)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "IdCdR ayant réalisé cette recette : " + valueAsString + " \n";
                    }
                    if (i == 7)
                    {
                        string valueAsString = reader.GetValue(i).ToString();
                        currentRowAsString += "Nombre de commandes de cette recette : " + valueAsString + " \n";
                    }
                }
                Console.WriteLine(currentRowAsString);

            }
            reader.Close();
            command.Dispose();
        }

        /// <summary>
        /// Permet de créer un nouveau client
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CreerClient(MySqlConnection maConnexion)
        {
            MySqlParameter nom = new MySqlParameter("@nom", MySqlDbType.VarChar);
            MySqlParameter numtel = new MySqlParameter("@numtel", MySqlDbType.VarChar);
            Console.WriteLine("Saisir le nom");
            nom.Value = Console.ReadLine();
            Console.WriteLine("Saisir le num de tel");
            numtel.Value = Console.ReadLine();
            string requete4 = "INSERT INTO `mydb`.`client`(`NumeroTelephone`,`Nom`,`idCDR`,`solde`) VALUES (@numtel,@nom,null,null);";
            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = requete4;
            command4.Parameters.Add(nom);
            command4.Parameters.Add(numtel);
            try
            {
                command4.ExecuteNonQuery();
                Console.WriteLine("Inscription validé, bienvenue ! ");
            }

            catch (MySqlException)

            {
                Console.WriteLine("ERREUR VOTRE NUMERO DE TELEPHONE EST DEJA ASSOCIE A UN COMPTE CLIENT.\nVEUILLEZ QUITTER L'APPLICATION ET VOUS CONNECTER EN TANT QUE CLIENT." );
                Console.WriteLine("\n\n\nAPPUYEZ SUR UNE TOUCHE POUR QUITTER L'APPLICATION ");
                Console.ReadKey(true);
                int compteur = 0;
                while (compteur < 3)
                {
                    int temps = 3 - compteur;
                    Console.Write(temps + "...");
                    compteur++;
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            }
        } 

        /// <summary>
        /// Permet de créer un nouveau cdr
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CreerCDR(MySqlConnection maConnexion)
        {
            MySqlParameter nom = new MySqlParameter("@nom", MySqlDbType.VarChar);
            MySqlParameter numtel = new MySqlParameter("@numtel", MySqlDbType.VarChar);
            MySqlParameter idCDR = new MySqlParameter("@idCDR", MySqlDbType.Int32);
            MySqlParameter solde = new MySqlParameter("@solde", MySqlDbType.Int32);
            Console.WriteLine("Saisir le nom");
            nom.Value = Console.ReadLine();
            Console.WriteLine("Saisir le num de tel");
            numtel.Value = Console.ReadLine();



            MySqlParameter id = new MySqlParameter("@id", MySqlDbType.VarChar);

            Random random = new Random();

            idCDR.Value = random.Next(0, 1000);
            MySqlParameter a = new MySqlParameter("@a", idCDR.Value);
            string requete2 = "select count(*) from  `mydb`.`client` where idCDR=@a;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            command2.Parameters.Add(a);

            while (command2.ExecuteNonQuery() != -1)
            {
                idCDR.Value = random.Next(0, 1000);
            }




            string requete4 = "INSERT INTO `mydb`.`client`(`NumeroTelephone`,`Nom`,`idCDR`,`solde`) VALUES (@numtel,@nom,@idCDR,0);";
            MySqlCommand command4 = maConnexion.CreateCommand();
            command4.CommandText = requete4;
            command4.Parameters.Add(nom);
            command4.Parameters.Add(numtel);
            command4.Parameters.Add(idCDR);


            try
            {
                command4.ExecuteNonQuery();
                Console.WriteLine("Inscriptions validé ! Votre idCDR : " + idCDR.Value);
            }
            catch (MySqlException)
            {

                Console.WriteLine("ERREUR VOTRE NUMERO DE TELEPHONE EST DEJA ASSOCIE A UN COMPTE CLIENT.\nVEUILLEZ QUITTER L'APPLICATION ET VOUS CONNECTER EN TANT QUE CLIENT.");
                Console.WriteLine("\n\n\nAPPUYEZ SUR UNE TOUCHE POUR QUITTER L'APPLICATION ");
                Console.ReadKey(true);
                int compteur = 0;
                while (compteur < 3)
                {
                    int temps = 3 - compteur;
                    Console.Write(temps + "...");
                    compteur++;
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);

            }





        } 

        /// <summary>
        /// Permet de créer une nouvelle recette
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CreerRecette(MySqlConnection maConnexion) 
        {
            MySqlParameter idRecette = new MySqlParameter("@idRecette", MySqlDbType.Int32);
            MySqlParameter type = new MySqlParameter("@type", MySqlDbType.VarChar);
            MySqlParameter nomRecette = new MySqlParameter("@nomRecette", MySqlDbType.VarChar);
            MySqlParameter nbreIngredients = new MySqlParameter("@nbreIngredients", MySqlDbType.VarChar);
            MySqlParameter descriptif = new MySqlParameter("@descriptif", MySqlDbType.VarChar);
            MySqlParameter prixdebase = new MySqlParameter("@prixdebase", MySqlDbType.VarChar);
            MySqlParameter id = new MySqlParameter("@id", MySqlDbType.VarChar);

            Random random = new Random();

            idRecette.Value = random.Next(0, 1000);
            int hasard = Convert.ToInt32(idRecette.Value);
            MySqlParameter a = new MySqlParameter("@a", idRecette.Value);
            string requete2 = "select count(*) from  `mydb`.`recette` where idrecette=@a;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            command2.Parameters.Add(a);
            //Console.WriteLine(command2.ExecuteNonQuery());
            while (command2.ExecuteNonQuery() != -1)
            {
                idRecette.Value = random.Next(0, 1000);
            }


            Console.WriteLine("Saisir type");
            Console.WriteLine("\nA: Plat");
            Console.WriteLine("\nB: Dessert");
            Console.WriteLine("\nC: Boisson");
            Console.WriteLine("\nD : Autre (précisez) :");
            ConsoleKey saisietype = Console.ReadKey(true).Key;
            switch (saisietype)
            {
                case ConsoleKey.A:
                    type.Value = "Plat";
                    Console.WriteLine("\nVous allez créer un Plat. Appuyez sur une touche pour continuer.\n");
                    Console.ReadKey(true);
                    break;
                case ConsoleKey.B:
                    type.Value = "Dessert";
                    Console.WriteLine("\nVous allez créer un Dessert. Appuyez sur une touche pour continuer.\n");
                    Console.ReadKey(true);
                    break;
                case ConsoleKey.C:
                    type.Value = "Boisson";
                    Console.WriteLine("\nVous allez créer une Boisson. Appuyez sur une touche pour continuer.\n");
                    Console.ReadKey(true);
                    break;
                case ConsoleKey.D:
                    type.Value = Console.ReadLine();
                    Console.WriteLine("\nVous allez créer un " + type.Value + ". Appuyez sur une touche pour continuer.\n");
                    Console.ReadKey(true);
                    break;
                    
            }

            //type.Value = Console.ReadLine();
            Console.WriteLine("Saisir le Nom de la Recette");
            nomRecette.Value = Console.ReadLine();
            Console.WriteLine("Saisir le nombre d'Ingredients");
            nbreIngredients.Value = SaisieNombre();
            int s = Convert.ToInt32(nbreIngredients.Value);
            Console.WriteLine("Saisir les " + s + " ingrédients nécessaires");
            List<string> ingredients = new List<string>();
            List<int> quantity = new List<int>();
            for (int i = 0; i < s; i++)
            {
                Console.WriteLine("Ingrédient n°" + (i+1));
                ingredients.Add(Console.ReadLine());
                Console.WriteLine("Saisir la quantité (en grammes) ?");
                int test = SaisieNombre();
                quantity.Add(test);
            }
            
            Console.WriteLine("Saisir le descriptif");
            descriptif.Value = Console.ReadLine();
            Console.WriteLine("Saisir le Prix De Base");
            //prixdebase.Value = Console.ReadLine();
            prixdebase.Value = SaisieDouble();
            Console.WriteLine("Saisir votre id");
            id.Value = SaisieNombre();

            string requete1 = "insert into `mydb`.`recette`(`idRecette`,`Type`,`NomRecette`,`NbreIngredients`,`Descriptif`,`Prix`,`idcdr`,`compteur`) VALUES (@idRecette,@type,@nomRecette,@nbreIngredients,@descriptif,@prixdebase,@id,0);";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete1;
            command1.Parameters.Add(idRecette);
            command1.Parameters.Add(type);
            command1.Parameters.Add(nomRecette);
            command1.Parameters.Add(nbreIngredients);
            command1.Parameters.Add(descriptif);
            command1.Parameters.Add(id);
            command1.Parameters.Add(prixdebase);
            try
            {
                command1.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {

                Console.WriteLine("Erreur" + e.ToString());

            }
            command1.Dispose();
            string requete3 = "insert into `mydb`.`estfait`(NomProduit,IdRecette,Quantite) VALUES (@nomproduit,@idrecette,@quantite);";
            MySqlParameter nomproduit = new MySqlParameter("@nomproduit", MySqlDbType.String);
            MySqlParameter quantite = new MySqlParameter("@quantite", MySqlDbType.Int32);
            //MySqlParameter idrecette2=new MySqlParameter()
            MySqlCommand command3 = maConnexion.CreateCommand();
            command3.CommandText = requete3;
            command3.Parameters.Add(nomproduit);
            command3.Parameters.Add(idRecette);
            command3.Parameters.Add(quantite);
            for (int i = 0; i < ingredients.Count; i++)
            {
                nomproduit.Value = ingredients[i];
                quantite.Value = quantity[i];
                command3.ExecuteNonQuery();
            }

            



        } 

        /// <summary>
        /// Permet de commander un plat
        /// </summary>
        /// <param name="maConnexion"></param>
        static void CreerCommande(MySqlConnection maConnexion)
        {
            MySqlParameter idcommande = new MySqlParameter("@idcommande", MySqlDbType.Int32);
            MySqlParameter numtel1 = new MySqlParameter("@numtel1", MySqlDbType.VarChar);
            MySqlParameter idrecette = new MySqlParameter("@idrecette", MySqlDbType.VarChar);
            MySqlParameter date = new MySqlParameter("@idrecette", MySqlDbType.Date);

            Console.WriteLine("Saisir votre numéro de téléphone");
            numtel1.Value = Console.ReadLine();
            Console.WriteLine("Saisir l'id de la recette");
            idrecette.Value = Console.ReadLine();
            Random random = new Random();

            idcommande.Value = random.Next(0,1000);

            date.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"Date de votre commande :  {date.Value} ");

            # region MAJ du compteur
            string requete = " update `mydb`.`recette` set compteur=compteur+1 where idRecette=@idrecette;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(idrecette);
            command.ExecuteNonQuery();
            command.Dispose();
            
            #endregion

            #region updates des prix

            //On recupère le compteur
            string requete3 = "select compteur from `mydb`.`recette` where idrecette=@idrecette;";
            MySqlCommand command3 = maConnexion.CreateCommand();
            command3.CommandText = requete3;
            command3.Parameters.Add(idrecette);

            MySqlDataReader reader = command3.ExecuteReader();
            reader.Read();
            //Console.WriteLine(reader["compteur"]);
            
            uint a = reader.GetUInt32("compteur");
            //Console.WriteLine(reader["compteur"]);
            Console.ReadKey();
            reader.Close();
            

            //Si le plat à été commandé 10 fois on augmente les cook de 2
            if (a == 10)
            {
                string requete4 = "update `mydb`.`recette` set  prix=prix+2.0 where idrecette=@idrecette;";
                MySqlCommand command4 = maConnexion.CreateCommand();
                command4.CommandText = requete4;
                command4.Parameters.Add(idrecette);
                command4.ExecuteNonQuery();
            }
            //si le plat a été commandé 50 fois on augmente les cook de 5
            else
            {
                if (a == 50)
                {
                    string requete5 = "update `mydb.`recette` set prix=prix+5.0 where idrecette=@idrecette;";
                    MySqlCommand command5 = maConnexion.CreateCommand();
                    command5.CommandText = requete5;
                    command5.Parameters.Add(idrecette);
                    command5.ExecuteNonQuery();
                }
            }
            #endregion

            #region Updates des stock de produit 

            string requete6 = "select nomproduit,quantite from estfait where idrecette=@idrecette;";
            MySqlCommand command6 = maConnexion.CreateCommand();
            command6.CommandText = requete6;
            command6.Parameters.Add(idrecette);
            MySqlDataReader reader6 = command6.ExecuteReader();
            List<string> nomproduit = new List<string>();
            List<string> qte = new List<string>();
            while (reader6.Read())
            {
                nomproduit.Add(reader6.GetString(0));
                qte.Add(reader6.GetString(1));
            }
            reader6.Close();
            command6.Dispose();
            for (int i = 0; i < nomproduit.Count; i++)
            {
                Console.WriteLine(nomproduit[i] + " " + qte[i]);

            }
            string requete7 = "update  produit set stockactuel=stockactuel-@quantite where nomproduit=@nomproduit;";
            MySqlParameter quantite = new MySqlParameter("@quantite", MySqlDbType.Int32);
            MySqlParameter nomprod = new MySqlParameter("@nomproduit", MySqlDbType.VarChar);
            MySqlCommand command7 = maConnexion.CreateCommand();
            command7.CommandText = requete7;
            command7.Parameters.Add(quantite);
            command7.Parameters.Add(nomprod);
            for (int i = 0; i < nomproduit.Count; i++)
            {
                quantite.Value = Convert.ToInt32(qte[i]);
                nomprod.Value = nomproduit[i];
                try
                {
                    command7.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            Console.WriteLine("Stock MAJ ! ");
            #endregion

            #region Payement des CDR

            string requete8 = "select idcdr from recette where idrecette=@idrecette";
            MySqlCommand command8 = maConnexion.CreateCommand();
            command8.Parameters.Add(idrecette);
            command8.CommandText = requete8;
            MySqlDataReader reader8 = command8.ExecuteReader();
            reader8.Read();
            MySqlParameter idcdr = new MySqlParameter("@idcdr", MySqlDbType.VarChar);
            string requete9 = "select numerotelephone from client where idCDR=@idcdr";
            MySqlCommand command9 = maConnexion.CreateCommand();
            command9.CommandText = requete9;
            command9.Parameters.Add(idcdr);
            idcdr.Value = reader8.GetString(0);
            reader8.Close();
            MySqlDataReader reader9 = command9.ExecuteReader();

            reader9.Read();


            MySqlParameter numtel = new MySqlParameter("@numtel", MySqlDbType.VarChar);
            numtel.Value = reader9.GetString(0);
            reader9.Close();

            string requete10 = "update client set solde=solde+2 where numerotelephone=@numtel";
            MySqlCommand command10 = maConnexion.CreateCommand();
            command10.Parameters.Add(numtel);
            command10.CommandText = requete10;
            try
            {
                command10.ExecuteNonQuery();
                Console.WriteLine("Cdr payé ! ");
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            #endregion

            MySqlCommand command11 = maConnexion.CreateCommand();
            command11.Parameters.Add(idrecette);
            command11.Parameters.Add(numtel1);
            command11.Parameters.Add(idcommande);
            Random ran = new Random();

            idcommande.Value = ran.Next(5, 1000);
            command11.CommandText = "INSERT into commande(idcommande,client_numerotelephone,idrecette,Date) VALUES(@idcommande,@numtel1,@idrecette,now()); ";
            try
            {
                command11.ExecuteNonQuery();
                Console.WriteLine("Commande effectuée !");
                Console.ReadKey();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }


        }  

        /// <summary>
        /// Permet de déterminer la recette la plus vendue cette semaine
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Toprecette(MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = "select idrecette from commande where datediff(now(),date)<=7;";
            MySqlDataReader reader = command.ExecuteReader();

            List<int> id = new List<int>();
            reader.Read();
            do
            {
                id.Add(Convert.ToInt32(reader.GetString(0)));
            } while (reader.Read() == true);

            reader.Close();
            for (int i = 0; i < id.Count; i++)
            {
                Console.WriteLine(id[i]);
                //Console.WriteLine(id.Count);
            }
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = "select idcdr from recette where idrecette=@idRecette;";
            MySqlParameter idRecette = new MySqlParameter("@idRecette", MySqlDbType.Int32);
            command1.Parameters.Add(idRecette);
            MySqlDataReader reader1 = command1.ExecuteReader();
            List<int> idcdr = new List<int>();
            idRecette.Value = 1;
            Console.WriteLine(reader1.Read());


        }

        /// <summary>
        /// Permet de déterminer les 5 recettes les plus vendues
        /// </summary>
        /// <param name="maConnexion"></param>
        static void Toprecettes(MySqlConnection maConnexion)
        {
            string requete_recette = "select * from recette order by compteur desc;";
            MySqlCommand command11 = maConnexion.CreateCommand();
            command11.CommandText = requete_recette;
            MySqlDataReader reader11 = command11.ExecuteReader();
            Console.WriteLine("\nVoici le top 5 des recettes :\n");
            int compt = 1;

            while (reader11.Read())
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i % 5 == 0)
                    {
                        Console.WriteLine("Le plat n°" + compt);
                        Console.WriteLine(reader11.GetValue(2).ToString());
                        Console.WriteLine("A été vendu " + reader11.GetValue(7).ToString() + " fois\n");
                        compt++;

                    }

                }

            }

            reader11.Close();
            command11.Dispose();


        }

        /// <summary>
        /// Enregistrer sous un dossier XML
        /// </summary>
        static void Write_XML(MySqlConnection maConnexion)
        {
            XmlDocument docxml = new XmlDocument();

            //Création de l'élément racine qu'on ajoute au document
            XmlElement racine = docxml.CreateElement("Liste");
            docxml.AppendChild(racine);

            //Création et insertion de l'en-tête XML (no <=> pas de DTD associée)
            XmlDeclaration xmldecl = docxml.CreateXmlDeclaration("1.0", "UTF-8", "no");
            docxml.InsertBefore(xmldecl, racine);

            

            string requete1 = "select * from recette ;";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete1;
            MySqlDataReader reader1 = command1.ExecuteReader();

            while (reader1.Read())
            {
                XmlElement autreBalise = docxml.CreateElement("Recette");
                racine.AppendChild(autreBalise);

                XmlElement un = docxml.CreateElement("Nom_Recette");
                un.InnerText = reader1.GetValue(2).ToString();
                autreBalise.AppendChild(un);

                XmlElement deux = docxml.CreateElement("Nombre_d_Ingredients");
                deux.InnerText = reader1.GetValue(3).ToString();
                autreBalise.AppendChild(deux);

                XmlElement trois = docxml.CreateElement("Descriptif");
                trois.InnerText = reader1.GetValue(4).ToString();
                autreBalise.AppendChild(trois);

                XmlElement quatre = docxml.CreateElement("Prix");
                quatre.InnerText = reader1.GetValue(5).ToString();
                autreBalise.AppendChild(quatre);

                XmlElement cinq = docxml.CreateElement("IdCDR");
                cinq.InnerText = reader1.GetValue(6).ToString();
                autreBalise.AppendChild(cinq);

                XmlElement six = docxml.CreateElement("Compteur");
                six.InnerText = reader1.GetValue(7).ToString();
                autreBalise.AppendChild(six);

            }
            reader1.Close();
            command1.Dispose();

            //Enregistrement du document XML --> à retrouver dans le dossier bin\debug du fichier source
            docxml.Save("Liste_Recettes.xml");
            Console.WriteLine("Liste de recettes XML créée.");
            Console.WriteLine("\nVoulez-vous afficher la liste des recettes au format XML ?");
            Console.WriteLine("[O] : Oui");
            Console.WriteLine("[N] : Non");
            ConsoleKey saisietype = Console.ReadKey(true).Key;
            switch (saisietype)
            {
                case ConsoleKey.O:
                    Process.Start("C:/Users/SELMI Bilal/source/repos/bdd_menu/bdd_menu/bin/Debug/Liste_Recettes.xml");
                    break;
                case ConsoleKey.N:
                    Console.Write("Retour au menu dans ");
                    int compteur = 0;
                    while (compteur < 3)
                    {
                        int temps = 3 - compteur;
                        Console.Write(temps + "...");
                        compteur++;
                        Thread.Sleep(1000);
                    }
                    break;
            }


        }

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #region Connexion;

            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;DATABASE=mydb;UID=root;PASSWORD=OCArina1+";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
                //Console.ReadKey();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur de connexion" + e.ToString());
                //Console.ReadKey();
                return;
                
            }

            #endregion

            //ConsoleKeyInfo cki;  
            
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            string Texte = "Bienvenue dans l'application de COOKING. Appuyez sur une touche pour continuer.";


            Console.WriteLine();
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (Texte.Length / 2)) + "}", Texte));
            Console.WriteLine("\n\n\n\n\n\n\n");
            Console.WriteLine("                 ___________   _____________   _____________   _     ___    _   _      _   ___________  ");
            Console.WriteLine("                |  _________| | ___________ | | ___________ | | |___/ //   | | | \\    | | |  _________| ");
            Console.WriteLine("                | |           | |         | | | |         | | |  ___ //    | | | \\\\   | | | |________   ");
            Console.WriteLine("================| |           | |         | | | |         | | |  |__ \\\\    | | | \\ \\\\ | | |  _______  |=============== ");
            Console.WriteLine("                | |_________  | |_________| | | |_________| | |  ____ \\\\   | | | |\\ \\\\| | | |_______| | ");
            Console.WriteLine("                |___________| |_____________| |_____________| |_|    \\\\\\\\  |_| |_| \\____| |___________| ");
            Console.WriteLine("                                                                                                               ");
            Console.WriteLine("                 _____________________________________________________________________________________ ");
            Console.WriteLine("                |_____________________________________________________________________________________|  ");
            Console.WriteLine("                                                                                                               ");
            Console.ReadKey();
            Console.Clear();
            bool f = false;
            do
            {
                Console.Clear();
                string etudiant1 = "Bilal SELMI";
                string etudiant2 = "Soulaïmane JOUHRI";
                string etudiant3 = "Cheick KANOUTE";
                Console.Write(etudiant1);
                Console.Write("\n" + etudiant2);
                Console.Write("\n" + etudiant3 + "\n");
                string mDate = DateTime.Now.ToString();
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth)) + "}", mDate));
                Console.WriteLine("QUE VOULEZ-VOUS FAIRE ?\n\n");
                f = false;
                Console.WriteLine("A: Se connecter en tant que client.");
                Console.WriteLine("B: S'inscrire en tant que nouveau client.\n");
                Console.WriteLine("C: Se connecter en tant que CDR (Créateur de recettes).");
                Console.WriteLine("D: S'inscrire en tant que CDR (Créateur de recettes).\n");
                Console.WriteLine("E: Afficher les recettes disponibles.\n\n");
                Console.WriteLine("F: Afficher le cdr de la semaine.");
                Console.WriteLine("G: Supprimer un cuisinier.");
                Console.WriteLine("H: Supprimer une recette.");
                Console.WriteLine("I: Afficher le CDR d'Or.");
                Console.WriteLine("J: Consultez vos Cooks");
                Console.WriteLine("K: Enregistrer la liste des recettes au format XML.");
                Console.WriteLine("\n\nL: --MODE DEMO--");
                Console.WriteLine("M: Accéder au site.");
                Console.WriteLine("\nS : Sortir. Quitter l'application.");
                ConsoleKey saisie = Console.ReadKey(true).Key;
                switch (saisie)
                {
                    case ConsoleKey.A:
                        Console.Clear();
                        Console.WriteLine("Vous avez choisi de vous connecter en tant que client...");
                        Console.WriteLine("----------------------------------------------------\n");
                        Thread.Sleep(1000);
                        ConnexionClient(maConnexion);
                        Console.WriteLine("\nVoulez-vous passer une commmande ?");
                        Console.WriteLine("Appuyez sur A si vous désirez passer une commande ou sur une autre touche pour revenir en arrière");
                        ConsoleKey creer1 = Console.ReadKey(true).Key;
                        if (creer1 == ConsoleKey.A)
                        {
                            CreerCommande(maConnexion);
                        }
                        break;
                    case ConsoleKey.B:
                        Console.Clear();
                        Console.WriteLine("Vous avez choisi de vous inscrire en tant que client...");
                        Console.WriteLine("----------------------------------------------------\n");
                        Thread.Sleep(1000);
                        CreerClient(maConnexion);
                        Console.WriteLine("\nVoulez-vous passer une commmande ?");
                        Console.WriteLine("Appuyez sur A si vous désirez passer une commande ou sur une autre touche pour revenir en arrière");
                        ConsoleKey creer2 = Console.ReadKey(true).Key;
                        if (creer2 == ConsoleKey.A)
                        {
                            CreerCommande(maConnexion);
                        }
                        break;
                    case ConsoleKey.C:
                        Console.Clear();
                        Console.WriteLine("Vous avez choisi de vous connecter en tant que CDR...");
                        Console.WriteLine("----------------------------------------------------\n");
                        Thread.Sleep(1000);
                        ConnexionClient(maConnexion);
                        Console.WriteLine("\nVoulez-vous créer une recette ?");
                        Console.WriteLine("Appuyez sur A si vous désirez créer une recette ou sur une autre touche pour revenir en arrière");
                        ConsoleKey creer = Console.ReadKey(true).Key;
                        if (creer == ConsoleKey.A)
                        {
                            CreerRecette(maConnexion);
                        }
                        break;
                    case ConsoleKey.D:
                        Console.Clear();
                        Console.WriteLine("Vous avez choisi de vous inscrire en tant que CDR...");
                        Console.WriteLine("----------------------------------------------------\n");
                        Thread.Sleep(1000);
                        CreerCDR(maConnexion);
                        Console.WriteLine("\n\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        Console.WriteLine("\nVoulez-vous créer une recette ?");
                        Console.WriteLine("Appuyez sur A si vous désirez créer une recette ou sur une autre touche pour revenir en arrière");
                        ConsoleKey creer3 = Console.ReadKey(true).Key;
                        if (creer3 == ConsoleKey.A)
                        {
                            CreerRecette(maConnexion);
                        }
                        break;
                    case ConsoleKey.E:
                        Console.Clear();
                        Console.WriteLine("Voici les recettes disponibles :");
                        Console.WriteLine("--------------------------------\n");
                        AfficherRecette(maConnexion);
                        Console.ReadKey();                       
                        break;
                    case ConsoleKey.F:
                        Console.Clear();
                        CDRdelasemaine(maConnexion);
                        Console.ReadKey();
                        break;
                    case ConsoleKey.G:
                        Console.Clear();
                        Console.WriteLine("Entrez le mot de passe administrateur pour pouvoir supprimer un Cuisinier :\n");
                        string mdp = Console.ReadLine();
                        if (mdp == "MotDePasse")
                        {
                            Console.WriteLine("Mot de passe correct.\n" );
                            Supprimercuisinier(maConnexion);
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Mot de passe incorrect. Appuyez sur une touche pour revenir au menu.");
                            Console.ReadKey();
                        }
                        
                        break;
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.WriteLine("Entrez le mot de passe administrateur pour pouvoir supprimer une Recette :\n");
                        string mdp2 = Console.ReadLine();
                        if (mdp2 == "MotDePasse")
                        {
                            Console.WriteLine("Mot de passe correct.\n");
                            Supprimerrecette(maConnexion);
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Mot de passe incorrect. Appuyez sur une touche pour revenir au menu.");
                            Console.ReadKey();
                        }

                        break;
                    case ConsoleKey.I:
                        Console.Clear();
                        CDROR(maConnexion);
                        Console.ReadKey();
                        break;
                    case ConsoleKey.J:
                        Console.Clear();
                        ConsulterCook(maConnexion);
                        Console.ReadKey();
                        break;
                    case ConsoleKey.K:
                        Console.Clear();
                        Write_XML(maConnexion);
                        break;
                    case ConsoleKey.L:
                        Console.Clear();
                        Console.WriteLine();
                        string bv = "Voici le mode démo:";
                        Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 3) + (Texte.Length / 3)) + "}", bv.ToUpper()));
                        string s = "--------------------";
                        Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 3) + (Texte.Length / 3)) + "}", s));
                        Console.WriteLine();
                        Console.WriteLine("Les informations suivant notre base de données seront affichées successivement chaque fois que vous appuierez sur une   touche quelconque.");
                        Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
                        string demo = "Appuyez sur une touche pour commencer le mode démo";
                        Console.WriteLine(String.Format("{0," + ((Console.WindowWidth /2) + (Texte.Length /3)) + "}", demo.ToUpper()));
                        Console.ReadKey(true);
                        Console.Clear();

                        Console.WriteLine("\nAffichage du nombre de clients :");
                        Console.WriteLine("----------------------------------");
                        string requete_client = "SELECT count(*) from `mydb`.`client`;";
                        MySqlCommand command = maConnexion.CreateCommand();
                        command.CommandText = requete_client;
                        MySqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        Console.WriteLine("\nLe nombre de clients dans notre base de données est : " + reader.GetValue(0).ToString());
                        reader.Close();
                        command.Dispose();

                        WriteAt("Appuyez sur une touche pour continuer", 0, 25);
                        Console.ReadKey(true);
                        Console.Clear();

                        Console.WriteLine("\nAffichage des informations CDR :");
                        Console.WriteLine("----------------------------------");
                        string requete_cdr = "SELECT count(*) from `mydb`.`client` where idCDR is not null;";
                        MySqlCommand command1 = maConnexion.CreateCommand();
                        command1.CommandText = requete_cdr;
                        MySqlDataReader reader1 = command1.ExecuteReader();
                        reader1.Read();
                        Console.WriteLine("\nLe nombre de CdR dans notre base de données est : " + reader1.GetValue(0).ToString());                        
                        reader1.Close();
                        command1.Dispose();

                        string requete_cdr_nom = "SELECT nom,idcdr from `mydb`.`client` where idCDR is not null;";
                        MySqlCommand command11 = maConnexion.CreateCommand();
                        command11.CommandText = requete_cdr_nom;
                        MySqlDataReader reader11 = command11.ExecuteReader();
                        string[] valuestring = new string[reader11.FieldCount];
                        Console.WriteLine("\nVoici leurs noms ainsi que leurs IdCDR :\n");
                        
                        while (reader11.Read())
                        {
                            for (int i = 0; i < reader11.FieldCount; i++)
                            {
                                valuestring[i] = reader11.GetValue(i).ToString();
                                Console.Write(valuestring[i] + " | ");
                                if (i % 2 != 0)
                                {
                                    Console.WriteLine();
                                }

                            }
                            
                        }

                        reader11.Close();
                        command11.Dispose();


                        Console.WriteLine("\n( NOM DU CDR ) --- ( NOMBRE DE CES RECETTES COMMANDEES )");
                        string requete_top = "select idcdr,sum(compteur) from recette group by idcdr ;";
                        MySqlCommand command111 = maConnexion.CreateCommand();
                        command111.CommandText = requete_top;
                        MySqlDataReader reader111 = command111.ExecuteReader();
                        Console.WriteLine("\nVoici leurs nombre de recettes commandées :\n");

                        while (reader111.Read())
                        {

                            for (int i = 0; i < reader111.FieldCount; i++)
                            {

                                if (i == 0)
                                {
                                    Console.WriteLine("IdCDR : " + reader111.GetValue(0).ToString());

                                }
                                if (i == 1)
                                {
                                    Console.WriteLine("Nombre recettes vendues : " + reader111.GetValue(1).ToString());

                                }
                            }
                        }
                        reader111.Close();
                        command111.Dispose();


                        WriteAt("Appuyez sur une touche pour continuer", 0, 5);
                        Console.ReadKey(true);
                        Console.Clear();

                        Console.WriteLine("\nAffichage du nombre de recettes :");
                        Console.WriteLine("---------------------------------");
                        string requete_recette = "SELECT count(*) from `mydb`.`client`;";
                        MySqlCommand command3 = maConnexion.CreateCommand();
                        command3.CommandText = requete_recette;
                        MySqlDataReader reader3 = command3.ExecuteReader();
                        reader3.Read();
                        int a = reader3.GetInt32(0) - 1;
                        Console.WriteLine("\nLe nombre de recettes dans notre base de données est : " + /*reader3.GetValue(0).ToString()*/ a);
                        reader3.Close();
                        command3.Dispose();

                        WriteAt("Appuyez sur une touche pour continuer", 0, 25);
                        Console.ReadKey(true);
                        Console.Clear();


                        string requete_pro = "select nomproduit from produit where stockactuel <= stockmin/2;";
                        MySqlCommand command5 = maConnexion.CreateCommand();
                        command5.CommandText = requete_pro;
                        MySqlDataReader reader5 = command5.ExecuteReader();
                        string[] valuestring3 = new string[reader5.FieldCount];
                        Console.WriteLine("\nVoici les produits dont le stock actuel est <= 2 * leur stock minimal :\n");
                        while (reader5.Read())
                        {
                            for (int i = 0; i < reader5.FieldCount; i++)
                            {
                                valuestring3[i] = reader5.GetValue(i).ToString();
                                Console.Write(valuestring3[i] + "\n");
                            }

                        }
                        reader5.Close();
                        command5.Dispose();

                        WriteAt("Appuyez sur une touche pour continuer", 0, 5);
                        Console.ReadKey(true);
                        Console.Clear();


                        MySqlParameter ing = new MySqlParameter("@ing", MySqlDbType.String);
                        Console.WriteLine("Veuillez saisir un des produits de la liste précédente : ");
                        ing.Value = Console.ReadLine();
                        Console.WriteLine("Voici les recettes contenant " + ing.Value +  " dans leur recette : \n");

                        
                        
                        MySqlCommand command8 = maConnexion.CreateCommand();
                        command8.Parameters.Add(ing);
                        command8.CommandText = "select recette.idRecette, recette.idCdr,recette.type, recette.NomRecette, recette.NbreIngredients, recette.Prix from recette, estfait where recette.IdRecette = estfait.IdRecette and estfait.nomProduit=@ing;";
                        

                        MySqlDataReader reader8 = command8.ExecuteReader();
                        string[] valuestring8 = new string[reader8.FieldCount];

                        try
                        {
                            while (reader8.Read())
                            {
                                /*for (int i = 0; i < reader8.FieldCount; i++)
                                {
                                    Console.WriteLine("Plat : ");
                                    Console.WriteLine(reader8.GetValue(3).ToString());
                                }*/
                                Console.WriteLine(reader8.GetValue(3).ToString());
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Aucun id ne correspond");
                        }
                        reader8.Dispose();

                        WriteAt("FIN DU MODE DEMO -- APPUYEZ SUR UNE TOUCHE POUR REVENIR AU MENU PRINCIPAL ", 0, 5);
                        Console.ReadKey(true);
                        int compteur = 0;
                        while (compteur <3)
                        {
                            int temps = 3 - compteur;
                            Console.Write(temps + "...");
                            compteur++;
                            Thread.Sleep(1000);                            
                        }
                        break;
                    /*case ConsoleKey.W:
                        Console.Clear();
                        CreerCommande(maConnexion);
                        break;*/
                    case ConsoleKey.M:
                        Process.Start("https://cooking-88.webself.net/");
                        break;
                    case ConsoleKey.S:
                        Environment.Exit(0);
                        break;

                }
            } while (!f);
        }
    }
}
