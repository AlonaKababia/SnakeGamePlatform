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
        TextLabel lblScore; TextLabel lblend;
        GameObject food;
        int points = 0, countEaten = 0;
        GameObject[] dog = new GameObject[2];
        const int BONUS = 5;
        const int SIZE = 40;
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
            lblScore = new TextLabel(points.ToString(), labelPosition);
            lblScore.SetFont("Ariel", 14);
            board.AddLabel(lblScore);
            Position labelEndPosition = new Position(300, 400);
            lblend = new TextLabel("Game Over!", labelEndPosition);
            lblend.SetFont("Ariel", 40);
            //Adding Game Object
           
            //DogHead
            Position dogPosition = new Position(200, 130);
            dog[0] = new GameObject(dogPosition, SIZE , SIZE);
            dog[0].SetImage(Properties.Resources.dog);
            dog[0].direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dog[0]);
            //dogBody
            Position dogBodyPosition = new Position(200, 100);
            dog[1] = new GameObject(dogBodyPosition, SIZE , SIZE);
            dog[1].SetImage(Properties.Resources.dogBody);
            dog[1].direction = GameObject.Direction.RIGHT;
            board.AddGameObject(dog[1]);
            //food
            Position foodPosition = new Position(200, 460);
            food = new GameObject(foodPosition, SIZE , SIZE);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(food);


            //Play file in loop!
            board.PlayBackgroundMusic(@"\Images\syria.mp4");
            //Play file once!
            board.SetBackgroundImage(Properties.Resources.BuckinghamPalace);


            //Start game timer!
            board.StartTimer(200);
            
        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            //eating
            bool isEating = dog[0].IntersectWith(food);
            if (isEating)
            {
                if(countEaten % BONUS == 0 && countEaten != 0)
                {
                    points += 30;
                    while (!CheckPos(dog, food))
                    {
                        food.SetPosition(RandomFoodPos(dog));
                    }
                    //set image to normal food here
                    dog = AddBody(dog, board);
                    countEaten++;
                }
                else
                {
                    points += 10;
                    while (!CheckPos(dog, food))
                    {
                        food.SetPosition(RandomFoodPos(dog));
                    }
                    if(countEaten % BONUS == BONUS - 1)
                    {
                        //set image to bonus food here
                    }
                    dog = AddBody(dog, board);
                    countEaten++;
                }
                lblScore.SetText(points.ToString());
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
                    dogPosition.Y = dogPosition.Y + SIZE;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.LEFT)
                {

                    dogPosition.Y = dogPosition.Y - SIZE;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.UP)
                {
                    dogPosition.X = dogPosition.X - SIZE;
                    dog[0].SetPosition(dogPosition);
                }
                if (dog[0].direction == GameObject.Direction.DOWN)
                {
                    dogPosition.X = dogPosition.X + SIZE;
                    dog[0].SetPosition(dogPosition);
                }
            //game over check
            if (SelfHit(board, dog))
            {
                board.StopTimer();
                board.AddLabel(lblend);
            }
        }
        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (key == (char)ConsoleKey.LeftArrow && dog[0].direction != GameObject.Direction.RIGHT)
                dog[0].direction = GameObject.Direction.LEFT;
            if (key == (char)ConsoleKey.RightArrow && dog[0].direction != GameObject.Direction.LEFT)
                dog[0].direction = GameObject.Direction.RIGHT;
            if (key == (char)ConsoleKey.UpArrow && dog[0].direction != GameObject.Direction.DOWN)
                dog[0].direction = GameObject.Direction.UP;
            if (key == (char)ConsoleKey.DownArrow && dog[0].direction != GameObject.Direction.UP)
                dog[0].direction = GameObject.Direction.DOWN;
        }
        //פעולה הבודקת פגיעה עצמית
        static bool SelfHit(Board board, GameObject[] dog)
        {
            for(int i = 1;i < dog.Length; i++)
            {
                if (dog[0].IntersectWith(dog[i]))
                {
                    return true;
                }
            }
            return false;
        }
        //פעולה המחזירה את המיקום החדש של האוכל
        static Position RandomFoodPos(GameObject[] dog)
        {
            Random rnd = new Random();
            int x = rnd.Next(1, 28);
            int y = rnd.Next(1, 38);
            Position pos = new Position(20*x, 20*y);
            return pos;
        }
        //פעולה המגדילה את המערך של הכלב
        static GameObject[] AddBody(GameObject[] dog, Board board)
        {
            GameObject[] newDog = new GameObject[dog.Length + 1];
            for (int i = 0; i < dog.Length; i++)
            {
                newDog[i] = dog[i];
            }
            newDog[dog.Length] = new GameObject(dog[dog.Length - 1].GetPosition(), SIZE , SIZE);
            newDog[dog.Length].SetImage(Properties.Resources.dogBody);
            newDog[dog.Length].direction = dog[dog.Length - 1].direction;
            board.AddGameObject(newDog[dog.Length]);
            return newDog;
        } 
        //פעולה הבודקת עם המיקום של האוכל מתנגש עם הכלב
        static bool CheckPos(GameObject[] dog, GameObject food)
        {
            for(int i = 0;i < dog.Length; i++) 
            {
                if (dog[i].IntersectWith(food))
                {
                    return false;
                }
            }
            return true;
        }
    } 
}
