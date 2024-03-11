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
        int points = 0, countEaten = 0;
        GameObject[] dog = new GameObject[2];


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
            Position labelPosition = new Position(0, 200);
            lblScore = new TextLabel("This is just an example! Use right and left arrows to change direction", labelPosition);
            lblScore.SetFont("Ariel", 14);
            board.AddLabel(lblScore);

            //Adding Game Object
           
            //DogHead
            Position dogPosition = new Position(200, 130);
            dog[0] = new GameObject(dogPosition, 20, 20);
            dog[0].SetImage(Properties.Resources.dog);
            dog[0].direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dog[0]);
            //dogBody
            Position dogBodyPosition = new Position(200, 100);
            dog[1] = new GameObject(dogBodyPosition, 20, 20);
            dog[1].SetImage(Properties.Resources.dogBody);
            dog[1].direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dog[1]);
            //food
            Position foodPosition = new Position(200, 460);
            food = new GameObject(foodPosition, 20, 20);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(food);


            //Play file in loop!
            board.PlayBackgroundMusic(@"\Images\backgroundMusic.mp4");
            //Play file once!
            board.SetBackgroundImage(Properties.Resources.greenBackground);


            //Start game timer!
            board.StartTimer(200);
            
        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            //game over check
            //eating
            bool isEating = dog[0].IntersectWith(food);
            if (isEating == true)
            {
                board.PlayShortMusic(@"\Images\eating.wav");
                Random rnd = new Random();
                int x = rnd.Next(0, 601);
                Position foodPosition = new Position(x, x);
                food = new GameObject(foodPosition, 20, 20);
                food.SetImage(Properties.Resources.food);
                food.direction = GameObject.Direction.RIGHT;
                board.AddGameObject(food);
                //points and golden prezel
                points = points + 100;
                countEaten++;
                if (countEaten % 5 == 0)
                {

                }
            }
            Position dogPosition;
            //movement
            for(int i = dog.Length - 1; i > 0 ; i--)
            {
                dog[i].SetPosition(dog[i-1].GetPosition());
            }
                dogPosition = dog[0].GetPosition();
                if (dog[0].direction == GameObject.Direction.RIGHT)
                {
                    dogPosition.Y = dogPosition.Y + 20;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.LEFT)
                {

                    dogPosition.Y = dogPosition.Y - 20;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.UP)
                {
                    dogPosition.X = dogPosition.X - 20;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.DOWN)
                {
                    dogPosition.X = dogPosition.X + 20;
                    dog[0].SetPosition(dogPosition);
                }
            
        }
        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (key == (char)ConsoleKey.LeftArrow)
                dog[0].direction = GameObject.Direction.LEFT;
            if (key == (char)ConsoleKey.RightArrow)
                dog[0].direction = GameObject.Direction.RIGHT;
            if (key == (char)ConsoleKey.UpArrow)
                dog[0].direction = GameObject.Direction.UP;
            if (key == (char)ConsoleKey.DownArrow)
                dog[0].direction = GameObject.Direction.DOWN;
        }
    }
}
