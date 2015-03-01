﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ouatelse.Managers;
using Ouatelse.Models;

namespace Ouatelse.Forms
{
    public partial class HolidayForm : Form
    {
        #region Les attributs de la classe
        //L'année courante à la construction de la forme
        private int currentYear = DateTime.Now.Year;

        //Liste des jours où l'on ne travaille pas cette années
        private List<DateTime> unworkingDate;

        //Nombre de jour déjà posés
        private int alreadyPresent;

        //Liste des congés déjà posés
        private List<Holiday> putHolidays; 

        //Liste des jours fériés de l'année courante
        private List<DateTime> permanentUnworkingDate;
        #endregion

        #region Constructeur de la forme
        public HolidayForm()
        {
            InitializeComponent();
            //On retire la séleciton par défaut
            this.holidays.ClearSelection();

            designCalendar();

            fillCalendar();

            
            this.year.Text = currentYear.ToString();
        }
        #endregion

        #region Méthode qui permet d'enregistrer tous les jours fériés de l'année courante
        public void fillPermanentUnworkingDate()
        {
            permanentUnworkingDate = new List<DateTime>();
            //On retire le 1er janvier
            permanentUnworkingDate.Add(new DateTime(currentYear, 1,1));
            //Le 1 mai
            permanentUnworkingDate.Add(new DateTime(currentYear, 5, 1));
            //Le 8 mai
            permanentUnworkingDate.Add(new DateTime(currentYear, 5, 8));
            //La fête nationale
            permanentUnworkingDate.Add(new DateTime(currentYear, 7, 14));
            //L'assomption
            permanentUnworkingDate.Add(new DateTime(currentYear, 8, 15));
            //La toussaint
            permanentUnworkingDate.Add(new DateTime(currentYear, 11, 1));
            //L'armistice
            permanentUnworkingDate.Add(new DateTime(currentYear, 11, 11));
            //Noël
            permanentUnworkingDate.Add(new DateTime(currentYear, 12, 25));

            //Calcul des jours non fixes ( algorithme de oudin ) 
            //Calcul du nombre d'or - 1
            int goldNumber = (int)(currentYear % 19);
            //Année diviser par 100
            int yearPerHundred = (int) (currentYear/100);
            // Calcul de l'epacte
            int epacte = (int)((yearPerHundred - yearPerHundred / 4 - (8 * yearPerHundred + 13) / 25 + (19 * goldNumber) + 15) % 30);
            //Le nombre de jours à partir du 21 mars pour atteindre la pleine lune Pascale
            int daysEquinoxeToMoonFull = (int)(epacte - (epacte / 28) * (1 - (epacte / 28) * (29 / (epacte + 1)) * ((21 - goldNumber) / 11)));
            //Jour de la semaine pour la pleine lune Pascale (0=dimanche)
            int weekDayMoonFull = (int)((currentYear + currentYear / 4 + daysEquinoxeToMoonFull + 2 - yearPerHundred + yearPerHundred / 4) % 7);
            // Nombre de jours du 21 mars jusqu'au dimanche de ou 
            // avant la pleine lune Pascale (un nombre entre -6 et 28)
            int daysEquinoxeBeforeFullMoon = daysEquinoxeToMoonFull - weekDayMoonFull;
            // mois de pâques
            int monthPaques = (int)(3 + (daysEquinoxeBeforeFullMoon + 40) / 44);
            // jour de pâques
            int dayPaques = (int)(daysEquinoxeBeforeFullMoon + 28 - 31 * (monthPaques / 4));
            // lundi de pâques
            DateTime mondayPaques = new DateTime(currentYear, monthPaques, dayPaques + 1);
            permanentUnworkingDate.Add(mondayPaques);
            // Ascension
            permanentUnworkingDate.Add(mondayPaques.AddDays(38));
            //Pentecote
            permanentUnworkingDate.Add(mondayPaques.AddDays(49));
        }
        #endregion

        #region Méthode qui dit si le jour testé est un jour ouvrable
        public bool isWorkingDate(DateTime date)
        {
            //S'il est dans la liste des jours fériés permanents ou des dimanches.
            if (permanentUnworkingDate.Contains(date) || date.DayOfWeek == DayOfWeek.Sunday)
            {
                unworkingDate.Add(date);
                return false;
            }
            return true;
        }
        #endregion

        #region Méthode pour mettre en forme le calendrier
        public void designCalendar()
        {
            //On fixe la largeur de la colonne des noms des mois
            this.holidays.RowHeadersWidth = 100;

            //On fixe le design de l'entête du tableau
            foreach (DataGridViewColumn col in this.holidays.Columns)
            {
                col.Width = 30;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        #endregion

        #region Méthode qui permet de remplir le calendrier au chargement
        public void fillCalendar()
        {
            //On remplit d'abord toutes les dates fériées de l'année courante
            fillPermanentUnworkingDate();

            //La liste des jous non ouvrables de l'année courante
            unworkingDate = new List<DateTime>();

            //On test la possibilté d'aller voir le planing de l'année précédente
            preventPreviousYear();

            //Boucle pour gérer les mois
            for (int i = 1; i <= 12; ++i)
            {
                DataGridViewRow row = new DataGridViewRow();
                //On ajoute le mois courant
                this.holidays.Rows.Add(row);

                //On met en forme la colonne "légende"
                row.HeaderCell.Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                //Boucle pour gérer les jours dans le mois courant.
                for (int j = 1; j <= 31; ++j)
                {
                    //On récupère la cellule courante.
                    DataGridViewCell cell = holidays.Rows[i - 1].Cells[j - 1];

                    //On vérifie que l'on ne dépasse pas le nombre de jour du mois en cours sinon on le grise
                    if (j <= DateTime.DaysInMonth(currentYear, i))
                    {
                        //On définit le style d'alignement de la cellule courante.
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        //On récupère la date liée à la cellule courante.
                        DateTime dateValue = new DateTime(currentYear, i, j);

                        //On récupère la première lettre du nom du jour
                        string day = dateValue.ToString("ddd", new CultureInfo("fr-FR")).Substring(0, 1);

                        //On met à jour le contenu de la cellule
                        cell.Value = day;
                        
                        //On vérifie si c'est un jour ouvrable, si ce n'en est pas un on le met en rouge
                        if (!isWorkingDate(dateValue))
                        {
                            cell.Style.Font = new Font("Arial", 8, FontStyle.Bold);
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        cell.ReadOnly = true;
                        cell.Style.BackColor = Color.Gray;
                    }
                }
            }
            preventSortingColumns();

            //On cherche à afficher les jours déjà posés
            UpdateAlreadyPost();
        }
        #endregion

        #region Affichage des jours posés en tant que congés
        public void UpdateAlreadyPost()
        {
            alreadyPresent = 0;
            putHolidays = new List<Holiday>();
            putHolidays = HolidayManager.Instance.Filter("WHERE salaries_id =" + AuthManager.Instance.User.Id + " AND " + currentYear + " = YEAR(date_debut)" ).ToList();
            Color day;
            DateTime current;
            foreach (Holiday holiday in putHolidays)
            {
                day = holiday.Accepted ? Color.Green : Color.Orange;
                current = holiday.StartingDate;
                for (int i = 0; i <= holiday.numberOfDays(); ++i)
                {
                    if (isWorkingDate(current))
                        alreadyPresent++;
                    holidays.Rows[current.Month - 1].Cells[current.Day - 1].Style.BackColor = day;
                    current = current.AddDays(1);
                }
            }
            this.nbPut.Text = alreadyPresent.ToString();
            this.nbRest.Text = (30 - alreadyPresent).ToString();
        }
        #endregion

        #region Méthode utilisée lors de la modification du calendrier
        public void updateCalendar()
        {
            unworkingDate = new List<DateTime>();
            fillPermanentUnworkingDate();

            preventPreviousYear();
            for (int i = 1; i <= 12; ++i)
            {
                for (int j = 1; j <= 31; ++j)
                {
                    DataGridViewCell cell = holidays.Rows[i-1].Cells[j-1];
                    cell.Style.BackColor = Color.White;
                    cell.Style.ForeColor = Color.Black;
                    if (j <= DateTime.DaysInMonth(currentYear, i))
                    {
                        DateTime dateValue = new DateTime(currentYear, i, j);
                        string day = dateValue.ToString("ddd", new CultureInfo("fr-FR")).Substring(0, 1);
                        cell.Value = day;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        if (day.Equals("s") || day.Equals("d"))
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            cell.Style.Font = new Font("Arial", 8, FontStyle.Bold);
                        }
                    }
                    else
                    {
                        cell.Value = "";
                        cell.ReadOnly = true;
                        cell.Style.BackColor = Color.Gray;
                    }
                }
            }
            UpdateAlreadyPost();
        }
        #endregion

        #region On annule le tri des colonnes
        public void preventSortingColumns()
        {
            foreach (DataGridViewColumn c in holidays.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        #endregion

        #region On vérifie la possibilité de passer à une année antérieure
        public void preventPreviousYear()
        {
            if (currentYear == DateTime.Now.Year)
                this.previousYear.Enabled = false;
            else
                this.previousYear.Enabled = true;
        }
        #endregion

        #region Méthode pour trier une liste de date
        static List<DateTime> SortAscending(List<DateTime> list)
        {
            list.Sort((a, b) => a.CompareTo(b));
            return list;
        }
        #endregion

        #region Méthode pour gérer l'ajout de congés
        private void newholiday_Click(object sender, EventArgs e)
        {
            //On vérifie directemrnt le nombre de jour déjà posé
            if (alreadyPresent == 30)
            {
                Utils.Error("Vous avez déjà posé tous vos jours de congés");
                return;
            }
            
            
            String data = "";
            List<DateTime> holidaysSorted = new List<DateTime>();

            //Utilisation de link, on récupère tous les jours sélectionnés
            List<DateTime> holidaysSelected = (from DataGridViewCell c in holidays.SelectedCells
                                               where c.Style.BackColor != Color.Gray 
                                               select new DateTime(currentYear, c.RowIndex + 1, c.ColumnIndex + 1)).ToList();

            //On récupère l'ordre croissant des dates posées pour les congés
            holidaysSorted = SortAscending(holidaysSelected);

            foreach (DateTime dt in holidaysSorted)
            {
                data += String.Format("{0:D}",dt);
                data += "\n";
            }
            DateTime startingDate = holidaysSorted[0];
            DateTime endingDate = holidaysSorted.Last();
            if ((endingDate - startingDate).Days + 1  != holidaysSorted.Count)
            {
                Utils.Error("Vous devez choisir des jours consécutifs");
                return;
            }
            int workingDate = 0;
            for (DateTime p = startingDate; p <= endingDate; p = p.AddDays(1))
            {
                if (p.DayOfWeek != DayOfWeek.Sunday)
                    workingDate++;
            }
            if (workingDate + alreadyPresent > 30)
            {
                Utils.Error("Vous ne pouvez pas poser plus de congés que vous en avez le droit.");
                return;
            }

            int nbHollidays = holidaysSorted.Except(unworkingDate).ToList().Count;
            int amplitude = holidaysSorted.Count;
            if (new NewHolidaysForm(startingDate, endingDate, nbHollidays, amplitude,
                    30 - (workingDate + alreadyPresent)).ShowDialog() != DialogResult.OK) return;
            HolidayManager.Instance.Save(new Holiday(startingDate, endingDate, AuthManager.Instance.User));
            this.holidays.ClearSelection();
            updateCalendar();
            Utils.Info("Votre demande de congé a bien été prise en compte");
        }
        #endregion

        #region Méthode pour gérer le clique sur l'année précédente
        private void previousYear_Click(object sender, EventArgs e)
        {
            if (currentYear > DateTime.Now.Year)
            {
                currentYear--;
                updateCalendar();
                this.year.Text = currentYear.ToString();
            }
        }
        #endregion

        #region Méthode pour gérer le clique sur l'année suivante
        private void nextYear_Click(object sender, EventArgs e)
        {
            currentYear++;
            updateCalendar();
            this.year.Text = currentYear.ToString();
        }
        #endregion
    }
}

