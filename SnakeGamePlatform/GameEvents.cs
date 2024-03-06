using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using WMPLib;
using System.Deployment.Application;

namespace SnakeGamePlatform
{

    public class GameEvents : IGameEvents
    {
        //Define game variables here! for example...
        //GameObject [] snake;
        TextLabel lblScore;
        GameObject food;
        GameObject dogBody;
        GameObject dog;
        int points = 0, countEaten = 0;


        //This function is called by the game one time on initialization!
        //Here you should define game board resolution and size (x,y).
        //Here you should initialize all variables defined above and create all visual objects on screen.
        //You could also start game background music here.
        //use board Object to add game objects to the game board, play background music, set interval, etc...
        public void GameInit(Board board)
        {

            //Setup board size and resolution!
            Board.resolutionFactor = 1;
            board.XSize = 600;
            board.YSize = 800;

            //Adding a text label to the game board.
            Position labelPosition = new Position(100, 20);
            lblScore = new TextLabel("This is just an example! Use right and left arrows to change direction", labelPosition);
            lblScore.SetFont("Ariel", 14);
            board.AddLabel(lblScore);

            //Adding Game Object
            //dogBody
            Position dogBodyPosition = new Position(200, 100);
            dogBody = new GameObject(dogBodyPosition, 30, 30);
            dogBody.SetImage(Properties.Resources.dogBody);
            dogBody.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dogBody);
            //DogHead
            Position dogPosition = new Position(200, 130);
            dog = new GameObject(dogPosition, 30, 30);
            dog.SetImage(Properties.Resources.dog);
            dog.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dog);
            //food
            Position foodPosition = new Position(200, 450);
            food = new GameObject(foodPosition, 30, 30);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(food);


            //Play file in loop!
            board.PlayBackgroundMusic(@"\Images\gameSound.wav");
            //Play file once!
            board.PlayShortMusic(@"\Images\eat.wav");


            //Start game timer!
            board.StartTimer(50);
            {
                dogBody.SetImage(Properties.Resources.dogBody);
                dog.SetImage(Properties.Resources.dog);
                food.SetImage(Properties.Resources.food);
                //eating
                bool isEating = dog.IntersectWith(food);
                if(isEating==true)
                {
                    Random rnd= new Random();
                    int x = rnd.Next(0, 601);
                    foodPosition = new Position(x, x);
                    food = new GameObject(foodPosition, 30, 30);
                    food.SetImage(Properties.Resources.food);
                    food.direction = GameObject.Direction.RIGHT;
                    board.AddGameObject(food);
                    //points and golden prezel
                    points = points + 100;
                    countEaten++;
                    if (countEaten%5==0)
                    {

                    }

                }
            }
        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            Position dogPosition = dog.GetPosition();
            if (dog.direction == GameObject.Direction.RIGHT)
            {
                dogPosition.Y = dogPosition.Y + 5;
                dog.SetPosition(dogPosition);
            }
            if (dog.direction == GameObject.Direction.LEFT)
            {

                dogPosition.Y = dogPosition.Y - 5;
                dog.SetPosition(dogPosition);
            }
            if (dog.direction == GameObject.Direction.UP)
            { 
                dogPosition.X = dogPosition.X - 5;
                dog.SetPosition(dogPosition);
            }
            if (dog.direction == GameObject.Direction.DOWN)
            { 
                dogPosition.X = dogPosition.X + 5; 
                dog.SetPosition(dogPosition);
            }

        }

        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (key == (char)ConsoleKey.LeftArrow)
                dog.direction = GameObject.Direction.LEFT;
            if (key == (char)ConsoleKey.RightArrow)
                dog.direction = GameObject.Direction.RIGHT;
            if (key == (char)ConsoleKey.UpArrow)
               dog.direction = GameObject.Direction.UP;
            if (key == (char)ConsoleKey.DownArrow)
               dog.direction = GameObject.Direction.DOWN;
        }
    }
}
