using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace BattleShipCS
{
    public partial class Form1 : Form
    {
        PictureBox[] playerBoard = new PictureBox[100];
        PictureBox[] computerBoard = new PictureBox[100];
        string[] playerShots = Enumerable.Repeat("e", 100).ToArray();
        string[] computerShots = new string[100];
        List<int> pShotsAvailable = Enumerable.Range(0, 100).ToList();
        List<int> cShotsAvailable = Enumerable.Range(0, 100).ToList();
        List<int> positionsAvailable = Enumerable.Range(0, 100).ToList();
        int playerShip1, playerShip2, playerShip3;
        int computerShip1, computerShip2, computerShip3;
        int turn = 0;
        bool isVertical;
        public enum Position { Vertical, Horizontal };
        Position position;
        bool inRange = true;

        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            PrintBoard();
            //playerShip1 = PlaceBoats(isVertical: true); //Using Named arguments
            try
            {
                PlaceBoats(Position.Vertical); //Using enum
                /*playerShip2 = PlaceBoats(Position.Horizontal);
                playerShip3 = PlaceBoats(Position.Horizontal);*/
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }


            RandomAssignments();
            foreach (var (tile, index) in computerBoard.Select((Name, index) => (Name, index)))
            {
                tile.Cursor = Cursors.Hand;
                tile.Click += Tile_Click;
            }
        }
        public void PlaceBoats(Position position)
        {

            foreach (var (tile, index) in playerBoard.Select((Name, index) => (Name, index)))
            {
                /*tile.MouseHover += (sender, EventArgs) => { Tile_MouseHover(sender, EventArgs, position); };*/
                tile.Click += (sender, EventArgs) => { Place_Boat(sender, EventArgs, position); };
                //return index;
            }
            return -1;
            //return index;
            /*else if (position == Position.Horizontal)
            {
                return index, index + 1;
            }*/

        }

        public void Place_Boat(object sender, EventArgs e, Position position)
        {
            //throw new NotImplementedException();

            int index = Array.IndexOf(playerBoard, sender);
            Console.WriteLine($"index {index} on playerBoard");
            if (position == Position.Vertical)
            {
                if (index < 90)
                {
                    playerBoard[index].Image = Properties.Resources.boatTop;
                    playerBoard[index + 10].Image = Properties.Resources.boatBottom;
                    playerShots[index] = "b";
                    playerShots[index + 10] = "b";

                }
                else
                {
                    inRange = false;
                    throw new IndexOutOfRangeException("The vertical boat won't fit. Play stay in a higher range");
                }
            }
            else if (position == Position.Horizontal)
            {
                playerBoard[index].Image = Properties.Resources.boatTop;
                playerBoard[index + 1].Image = Properties.Resources.boatBottom;
                playerShots[index] = "b";
                playerShots[index + 1] = "b";
            }
            ///return index;
        }

        /*        public void Tile_MouseHover(object sender, EventArgs e, Position position)
                {
                    //throw new NotImplementedException();
                    int index = Array.IndexOf(playerShots, sender);
                    if (index >= 0 && index < 90)
                    {
                        if (position == Position.Vertical)
                        {
                            playerBoard[index].Image = Properties.Resources.boatTop;
                            playerBoard[index + 10].Image = Properties.Resources.boatBottom;

                        }
                        Console.WriteLine($"index : {index}");
                    }

                }

        */
        public void WinCheck(string[] board)
        {
            int counter = 0;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == "h")
                {
                    counter++;
                }
            }
            if (counter == 6)
            {
                MessageBox.Show("Game over!");
            }
        }

        private void Tile_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            var tile = sender as PictureBox;
            int index = Array.IndexOf(computerBoard, tile);
            Console.WriteLine($"this is tile {tile.Name} at index {index}, identified as {computerShots[index]}");

            if (computerShots[index] == null)
            {
                computerShots[index] = "m"; //for miss
                tile.Image = Properties.Resources.splash;
            }
            else if (computerShots[index] == "b") //for boat
            {
                computerShots[index] = "h";
                tile.Image = Properties.Resources.hit;
                MessageBox.Show("Target hit!");
            }
            else if (computerShots[index] == "m")
            {
                MessageBox.Show("This target was already missed. Try another");
            }

            WinCheck(computerShots);
        }

        public void PrintBoard()
        {
            //player side - tiles 45x45, with a 10x10 grid.
            int posXstart = 50;
            int posYstart = 90;
            //foreach (PictureBox tile in playerBoard)
            for (int i = 0; i < playerBoard.Length; i++)
            {
                playerBoard[i] = new PictureBox
                {
                    Name = "tile" + i,
                    Location = new Point(posXstart + (i % 10) * 45, posYstart + (i / 10) * 45),
                    Size = new Size(44, 44),
                    BackgroundImage = Properties.Resources.water,
                    Visible = Enabled
                };
                this.Controls.Add(playerBoard[i]);


            }

            posXstart = 600;
            posYstart = 90;
            // computer side
            for (int i = 0; i < computerBoard.Length; i++)
            {
                computerBoard[i] = new PictureBox
                {
                    Name = "tile" + i,
                    Location = new Point(posXstart + (i % 10) * 45, posYstart + (i / 10) * 45),
                    Size = new Size(44, 44),
                    BackgroundImage = Properties.Resources.water,
                    Visible = Enabled
                };
                this.Controls.Add(computerBoard[i]);
            }
        }

        public void RandomAssignments()
        {
            //assign boat to random. and hits? And taking off from the positionsAvailable list.
            computerShip1 = AssignBoatsVertical();
            computerShip2 = AssignBoatsHorizontal();
            computerShip3 = AssignBoatsHorizontal();
            Debug.WriteLine($"position 1 : {computerShip1} \nposition 2 : {computerShip2} \nposition 3 : {computerShip3}");
            //Once we assigned the 2 different position, that will be different, we refill the list? If needed?
        }

        public int AssignBoatsHorizontal()
        {
            //assign a random value to the boat and remove it and its next one for the horizontal boat. For vertical, remove the one 10 after.
            Random boatPosition = new Random();
            int ship = boatPosition.Next(0, positionsAvailable.Count);
            if (computerShots[ship] != "b")
            {
                //to avoid the overlap on the edges.
                if (ship % 10 == 9)
                {
                    ship--;
                }
                computerBoard[ship].Image = Properties.Resources.boatLeft;
                computerShots[ship] = "b";
                computerBoard[ship + 1].Image = Properties.Resources.boatRight;
                computerShots[ship + 1] = "b";
                return positionsAvailable[ship];
            }
            else
            {
                return AssignBoatsHorizontal();
            }

        }

        public int AssignBoatsVertical()
        {

            //assign a random value to the boat position. If there are no boats, but if there is, we recurse through and recall the function. 
            Random boatPosition = new Random();
            int ship = boatPosition.Next(0, positionsAvailable.Count - 10);
            if (computerShots[ship] != "b")
            {
                computerBoard[ship].Image = Properties.Resources.boatTop;
                computerShots[ship] = "b";
                computerBoard[ship + 10].Image = Properties.Resources.boatBottom;
                computerShots[ship + 10] = "b";

                return positionsAvailable[ship];
            }
            else
            {
                return AssignBoatsVertical();
            }
        }

        public void RemovePosition(int ship)
        {
            positionsAvailable.Remove(ship);
            positionsAvailable.Remove(ship + 1);
            positionsAvailable.Remove(ship + 10);
        }
        public void ReturnPosition(int boat)
        {
            positionsAvailable.Add(boat);
        }

    }
}
