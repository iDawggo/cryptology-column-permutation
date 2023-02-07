using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Column_Permutation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string unfTxt; // public, used throughout the program, unformatted text
        public int txtLength; // public, used throughout the program, length of unformatted text

        private void inputTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorsTxtIO.Text = "";
            unfTxt = Regex.Replace(inputTxt.Text.ToLower(), "[^a-z]", ""); //unformatted ciphertext

            bool checkTxt = Regex.IsMatch(unfTxt, @"^[a-zA-Z]+$"); //checking if input isnt exclusively numbers or symbols
            if(checkTxt == false)
            {
                errorsTxtIO.Text = "Please input a valid string with letters! No numbers or symbols!";
                return;
            }

            txtLength = unfTxt.Length; //text length
        }

        private void calcPrVd_Click(object sender, RoutedEventArgs e)
        {
            errorsRectVd.Text = "";
            if (txtLength == 0 || string.IsNullOrEmpty(unfTxt)) //error checking for empty text in text box
            {
                errorsRectVd.Text = "Please input valid ciphertext in 'Input and Output!'";
            }
            else //else... perform the actual method.
            {
                List<Rectangles> rectList = new List<Rectangles>(); //list of possible rectangles
                for (int i = 1; i <= txtLength; i++) //loops through text
                {
                    if (txtLength % i == 0) //checks if i is a multiple based off the value of txtLength
                    {
                        Rectangles temp = new Rectangles(); //if a multiple, a rectangle is created
                        {
                            temp.rowNumbers = (int)(i);
                            temp.columnnNumbers = (int)(txtLength / i);
                            temp.vowelDifference = (double)(vowelDiff(temp.rowNumbers, temp.columnnNumbers, unfTxt));
                        }
                        rectList.Add(temp); //adding the rectangle to the list
                    }
                }
                rectangleDg.ItemsSource = rectList; //outputs the list
            }
        }

        private void calcDataTab_Click(object sender, RoutedEventArgs e)
        {
            errorsTxtDataTab.Text = "";
            int numCol;
            int numRow;
            bool successNumCol = Int32.TryParse(numColTxt.Text, out numCol); //pulls and parses input into an int if correct
            if (successNumCol == false || numCol == 0 || txtLength % numCol != 0) //error checking for correct input for num of columns
            {
                errorsTxtDataTab.Text = "Please input a valid integer > 0!";
                return;
            }
            else if (string.IsNullOrEmpty(unfTxt)) //additional error checking if the unformatted ciphertext exists
            {
                errorsTxtDataTab.Text = "Please input a valid string 'Input and Output!'";
            }
            else //else... perform the actual methods
            {
                /* * * * * * * * * *
                 * CIPHERTEXT GRID *
                 * * * * * * * * * */
                numRow = txtLength / numCol;
                DataTable txtTable = new DataTable(); //data table needed for laying out individual letters
                DataRow row;
                DataColumn column;

                for (int i = 1; i <= numCol; i++) //creates all columns based on the number of columns inputted
                {
                    column = new DataColumn();
                    column.ColumnName = i.ToString();
                    txtTable.Columns.Add(column);
                }

                for (int i = 0; i < numRow; i++) //loops based on the number of rows calculated
                {
                    row = txtTable.NewRow(); //creates a row
                    int txtCounter = i; //makes sure text increments by 1
                    for (int j = 0; j < numCol; j++) //loops based on number of columns
                    {
                        if (txtCounter > unfTxt.Length - 1) //end condition for the loop
                        {
                            break;
                        }
                        row[j] = unfTxt[txtCounter]; //sets the row values to the character indexes
                        txtCounter += numRow; //appends the counter by numRow to make sure the counter correctly skips
                    }
                    txtTable.Rows.Add(row); //adds the row to the table
                }
                textDg.ItemsSource = txtTable.DefaultView; //outputs table

                /* * * * * * *
                 * CENTIBANS *
                 * * * * * * */
                List<Centibans> cipherCenti = new List<Centibans>(); //list of centibans created based on centibans class
                string[,] txtArray = new string[numRow, numCol]; //2D array of each string's char
                int arrayCount = 0;

                for (int i = 0; i < numCol; i++) //loop for adding all chars into the array
                {
                    for (int j = 0; j < numRow; j++)
                    {
                        txtArray[j, i] = unfTxt.Substring(arrayCount, 1);
                        arrayCount++;
                    }
                }

                for (int i = 0; i < numCol; i++) //loop for calculating centiban sums
                {
                    for (int j = 0; j < numCol; j++)
                    {
                        double centibanSum = 0.0;
                        for (int k = 0; k < numRow; k++)
                        {
                            if (i != j)
                            {
                                centibanSum += centiban(txtArray[k, j], txtArray[k, i]); //calculates sum
                            }
                        }
                        if (i != j)
                        {
                            Centibans centiTemp = new Centibans(); //constructs a centiban class
                            {
                                centiTemp.rightColumn = i + 1;
                                centiTemp.leftColumn = j + 1;
                                centiTemp.centibanSum = centibanSum;
                            }
                            cipherCenti.Add(centiTemp); //adds centiban to the list
                        }
                    }
                }
                centibanDg.ItemsSource = cipherCenti; //outputs list
            }
        }

        private void txtDecrypt_Click(object sender, RoutedEventArgs e)
        {
            outputTxt.Text = "Not done.";
        }

        double vowelDiff(int row, int col, string cipherTxt)
        {
            string[,] txtArray = new string[row, col]; //2D array of each char in the text

            int numCount = 0;
            double vDiff = 0.0;

            for(int i = 0; i < col; i++) //loop for adding all chars into the array
            {
                for(int j = 0; j < row; j++)
                {
                    txtArray[j, i] = cipherTxt.Substring(numCount, 1);
                    numCount++;
                }
            }

            for(int i = 0; i < row; i++) //loops throguh the text array, checking for vowels
            {
                double count = 0.0;
                for(int j = 0; j < col; j++)
                {
                    if(txtArray[i, j] == "a" || txtArray[i, j] == "e" || txtArray[i, j] == "i" || txtArray[i, j] == "o" || txtArray[i, j] == "u")
                    {
                        count++;
                    }
                }
                vDiff += Math.Abs(count - (col * 0.4)); //calculating vowel difference
            }
            return vDiff;
        }

        double centiban(string a, string b)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            double[,] centibanVals = new double[,] { { 0.331093, 0.459178, 0.615747, 0.737111, 0.128084, 0.384253, 0.459178, 0.256169, 0.651624, 0.128084, 0.256169, 0.768506, 0.615747, 0.89659, 0.256169, 0.587262, 0, 0.827352, 0.814303, 0.83954, 0.602053, 0.487663, 0.331093, 0, 0.587262, 0 }, { 0.256169, 0, 0, 0, 0.662186, 0, 0, 0, 0.256169, 0.128084, 0, 0.459178, 0.128084, 0, 0.384253, 0, 0, 0.256169, 0.128084, 0.128084, 0.256169, 0, 0, 0, 0.487663, 0 }, { 0.553571, 0, 0.331093, 0.128084, 0.768506, 0.128084, 0, 0.615747, 0.487663, 0, 0.384253, 0.425487, 0.128084, 0.128084, 0.814303, 0, 0, 0.384253, 0.128084, 0.615747, 0.384253, 0, 0.128084, 0, 0.128084, 0 }, { 0.640422, 0.384253, 0.384253, 0.512337, 0.774192, 0.512337, 0.256169, 0.256169, 0.737111, 0.128084, 0, 0.331093, 0.425487, 0.384253, 0.640422, 0.425487, 0.256169, 0.587262, 0.602053, 0.628496, 0.425487, 0.331093, 0.384253, 0, 0.128084, 0 }, { 0.656981, 0.384253, 0.768506, 0.884665, 0.818756, 0.662186, 0.384253, 0.487663, 0.737111, 0.128084, 0, 0.750316, 0.615747, 0.998343, 0.587262, 0.681656, 0.587262, 0.953325, 0.865195, 0.795334, 0.331093, 0.681656, 0.487663, 0.487663, 0.384253, 0.128084 }, { 0.297403, 0, 0.256169, 0.128084, 0.553571, 0.571183, 0.128084, 0, 0.805062, 0, 0, 0.256169, 0.128084, 0, 0.80974, 0.128084, 0, 0.534102, 0.331093, 0.571183, 0.331093, 0, 0.128084, 0, 0.128084, 0 }, { 0.359578, 0, 0.256169, 0.128084, 0.615747, 0.256169, 0.128084, 0.681656, 0.425487, 0.128084, 0, 0.256169, 0.128084, 0.331093, 0.459178, 0.256169, 0, 0.425487, 0.331093, 0.384253, 0.256169, 0, 0.128084, 0, 0, 0 }, { 0.553571, 0.128084, 0.331093, 0.256169, 0.681656, 0.425487, 0, 0, 0.774192, 0, 0, 0.128084, 0.256169, 0.331093, 0.681656, 0.128084, 0.128084, 0.651624, 0.384253, 0.743831, 0.512337, 0, 0.128084, 0, 0.128084, 0 }, { 0.384253, 0.256169, 0.699268, 0.459178, 0.602053, 0.553571, 0.672177, 0, 0, 0, 0.256169, 0.707482, 0.534102, 0.925899, 0.814303, 0.487663, 0, 0.737111, 0.785065, 0.737111, 0, 0.72289, 0, 0.628496, 0, 0.256169 }, { 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0 }, { 0, 0, 0.128084, 0, 0.459178, 0, 0, 0, 0.256169, 0, 0, 0.128084, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0, 0 }, { 0.384253, 0.331093, 0.331093, 0.534102, 0.795334, 0.331093, 0.128084, 0.128084, 0.681656, 0, 0, 0.737111, 0, 0.128084, 0.602053, 0.331093, 0, 0.256169, 0.459178, 0.512337, 0.256169, 0.256169, 0.256169, 0, 0.553571, 0 }, { 0.662186, 0.459178, 0.331093, 0.128084, 0.730137, 0.128084, 0, 0.128084, 0.534102, 0, 0, 0, 0.602053, 0, 0.553571, 0.512337, 0, 0.256169, 0.384253, 0.256169, 0.256169, 0, 0, 0, 0.256169, 0 }, { 0.602053, 0.331093, 0.672177, 0.858221, 0.875186, 0.534102, 0.737111, 0.384253, 0.75658, 0.128084, 0.256169, 0.425487, 0.425487, 0.512337, 0.662186, 0.331093, 0.128084, 0.384253, 0.715346, 0.942387, 0.487663, 0.331093, 0.331093, 0, 0.425487, 0 }, { 0.359578, 0.384253, 0.512337, 0.587262, 0.331093, 0.72289, 0.256169, 0.331093, 0.425487, 0.128084, 0.256169, 0.672177, 0.72289, 0.930762, 0.459178, 0.72289, 0, 0.89659, 0.615747, 0.672177, 0.795334, 0.487663, 0.512337, 0.128084, 0.256169, 0 }, { 0.487663, 0.128084, 0.128084, 0.128084, 0.707482, 0.256169, 0, 0.331093, 0.459178, 0, 0, 0.602053, 0.384253, 0.128084, 0.651624, 0.571183, 0, 0.662186, 0.459178, 0.512337, 0.331093, 0.128084, 0.128084, 0, 0.128084, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0.628496, 0, 0, 0, 0, 0 }, { 0.676977, 0.256169, 0.534102, 0.651624, 0.975325, 0.459178, 0.487663, 0.331093, 0.75658, 0.128084, 0.128084, 0.425487, 0.534102, 0.487663, 0.743831, 0.602053, 0, 0.571183, 0.762639, 0.818756, 0.425487, 0.425487, 0.384253, 0, 0.534102, 0 }, { 0.587262, 0.331093, 0.602053, 0.425487, 0.847241, 0.587262, 0.256169, 0.730137, 0.779709, 0, 0.128084, 0.256169, 0.331093, 0.384253, 0.628496, 0.553571, 0, 0.425487, 0.672177, 0.89368, 0.571183, 0.128084, 0.384253, 0, 0.128084, 0 }, { 0.615747, 0.331093, 0.459178, 0.459178, 0.915771, 0.487663, 0.128084, 0.933146, 0.831505, 0, 0, 0.425487, 0.459178, 0.487663, 0.850974, 0.256169, 0.128084, 0.651624, 0.672177, 0.672177, 0.425487, 0, 0.790271, 0, 0.814303, 0.128084 }, { 0.297403, 0.331093, 0.331093, 0.331093, 0.571183, 0.128084, 0.512337, 0, 0.425487, 0, 0, 0.459178, 0.425487, 0.690671, 0.128084, 0, 0, 0.762639, 0.587262, 0.587262, 0, 0.128084, 0, 0, 0, 0 }, { 0.331093, 0, 0, 0, 0.875186, 0, 0, 0, 0.587262, 0, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0 }, { 0.459178, 0, 0, 0, 0.699268, 0, 0, 0.384253, 0.602053, 0, 0, 0.128084, 0, 0.256169, 0.672177, 0, 0, 0.128084, 0.128084, 0, 0, 0, 0, 0, 0.128084, 0 }, { 0.128084, 0, 0.256169, 0.128084, 0.128084, 0.128084, 0, 0.128084, 0.256169, 0, 0, 0, 0, 0.128084, 0.128084, 0.256169, 0, 0.128084, 0.128084, 0.487663, 0, 0, 0, 0, 0, 0 }, { 0.331093, 0.256169, 0.384253, 0.384253, 0.534102, 0.571183, 0.128084, 0.128084, 0.331093, 0, 0, 0.256169, 0.256169, 0.459178, 0.553571, 0.331093, 0, 0.384253, 0.571183, 0.628496, 0.128084, 0, 0.128084, 0, 0, 0 }, { 0, 0, 0, 0, 0.256169, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            //centiban values pulled from textfile

            double centiban = centibanVals[alphabet.IndexOf(a), alphabet.IndexOf(b)]; //calculates centiban values
            return centiban;
        }
    }

    public class Rectangles //class for creating columns in rectangle datagrid
    {
        public int rowNumbers { get; set; }
        public int columnnNumbers { get; set; }
        public double vowelDifference { get; set; }
    }

    public class Centibans //class for creating columns in the centiban datagrid
    {
        public int leftColumn { get; set; }
        public int rightColumn { get; set;}
        public double centibanSum { get; set; }
    }
}