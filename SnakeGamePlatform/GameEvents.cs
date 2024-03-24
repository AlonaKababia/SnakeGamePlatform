using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using WMPLib;
using System.Deployment.Application;
using System.Drawing.Drawing2D;

namespace SnakeGamePlatform
{
    public class GameEvents : IGameEvents
    {
        //Define game variables here! for example...
        //GameObject [] snake;
        TextLabel lblScore; TextLabel lblEnd; TextLabel lblPause; TextLabel lblStartCredit; TextLabel lblDifficulty; TextLabel lblStart;
        GameObject food;
        int points = 0, countEaten = 0;
        GameObject[] dog = new GameObject[2];
        GameObject upWall, leftWall, downWall, rightWall;
        const int BONUS = 5;
        const int SIZE = 40;
        int timerInterval = 200;
        bool isPaused = false, start = true;
        int timerCounter = 0;
        const int START = 20, MID = 160, HARD = 110;
        string difficulty = "difficulty: easy";
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
            Position creditPos = new Position(200, 50);
            lblStartCredit = new TextLabel("Yonatan Volsky, Itamar Goffer, Alona Kababia and Noa Avitov \r\n Present:", creditPos);
            lblStartCredit.SetFont("Ariel", 18);
            board.AddLabel(lblStartCredit);
            Position labelPosition = new Position(10, 200);
            lblScore = new TextLabel(points.ToString(), labelPosition);
            lblScore.SetFont("Ariel", 14);
            Position labelEndPosition = new Position(300, 400);
            lblEnd = new TextLabel("Game Over!", labelEndPosition);
            lblEnd.SetFont("Ariel", 40);
            Position labelDifPos = new Position(10, 250);
            lblDifficulty = new TextLabel(difficulty, labelDifPos);
            lblDifficulty.SetFont("Ariel", 14);
            //game objects
            //upwall
            Position upWallPos = new Position(0, 0);
            upWall = new GameObject(upWallPos, 800, 10);
            upWall.SetBackgroundColor(Color.Black);
            //leftwall
            Position leftWallPos = new Position(0, 0);
            leftWall = new GameObject(leftWallPos, 10, 600);
            leftWall.SetBackgroundColor(Color.Black);
            //rightwall
            Position rightWallPos = new Position(0, 775);
            rightWall = new GameObject(rightWallPos, 10, 600);
            rightWall.SetBackgroundColor(Color.Black);
            //downwall
            Position downWallPos = new Position(553, 0);
            downWall = new GameObject(downWallPos, 800, 10);
            downWall.SetBackgroundColor(Color.Black);
            //dog head
            Position dogPosition = new Position(200, 130);
            dog[0] = new GameObject(dogPosition, SIZE, SIZE);
            dog[0].SetImage(Properties.Resources.dog);
            dog[0].direction = GameObject.Direction.RIGHT;
            //dogBody
            Position dogBodyPosition = new Position(200, 100);
            dog[1] = new GameObject(dogBodyPosition, SIZE, SIZE);
            dog[1].SetImage(Properties.Resources.dogBody);
            dog[1].direction = GameObject.Direction.RIGHT;
            //food
            Position foodPosition = new Position(200, 460);
            food = new GameObject(foodPosition, SIZE, SIZE);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            //Start game timer!
            board.StartTimer(timerInterval);

        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            if (timerCounter == START + 1)
            {
                board.RemoveLabel(lblStartCredit); // after credit screen
                board.AddLabel(lblScore);
                board.AddLabel(lblDifficulty);
                board.AddGameObject(upWall); board.AddGameObject(leftWall); board.AddGameObject(downWall); board.AddGameObject(rightWall);
                board.AddGameObject(dog[0]);
                board.AddGameObject(dog[1]);
                board.AddGameObject(food);
                //Play file in loop!
                board.PlayBackgroundMusic(@"\Images\syria.mp4");
                //Play file once!
                board.SetBackgroundImage(Properties.Resources.BuckinghamPalace);
            }
            if (timerCounter > START)
            {
                if (start)
                {
                    board.StopTimer();
                    Position startGameSentence = new Position(10, 400);
                    lblStart = new TextLabel("Press the space bar of the game to start", startGameSentence);
                    lblStart.SetFont("Ariel", 14);
                    board.AddLabel(lblStart);
                }
                else
                {
                    //eating
                    bool isEating = dog[0].IntersectWith(food);
                    if (isEating)
                    {
                        if (countEaten % BONUS == 0 && countEaten != 0)
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
                            while (!CheckPos(dog, food) || !food.OnScreen(board))
                            {
                                food.SetPosition(RandomFoodPos(dog));
                            }
                            if (countEaten % BONUS == BONUS - 1)
                            {
                                //set image to bonus food here
                            }
                            dog = AddBody(dog, board);
                            countEaten++;
                        }
                        lblScore.SetText(points.ToString());
                        if (timerInterval > 75) // adding difficulty
                        {
                            timerInterval -= 5;
                            board.StartTimer(timerInterval);
                            if (timerInterval < MID) // changing difficulty label
                            {
                                difficulty = "difficulty: midium";
                                lblDifficulty.SetText(difficulty);
                            }
                            if (timerInterval < HARD)
                            {
                                difficulty = "difficulty: hard";
                                lblDifficulty.SetText(difficulty);
                            }
                        }
                    }
                }
                //movement
                Position dogPosition;
                for (int i = dog.Length - 1; i > 0; i--)
                {
                    dog[i].SetPosition(dog[i - 1].GetPosition());
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
                if (SelfHit(board, dog) || dog[0].IntersectWith(upWall) || dog[0].IntersectWith(leftWall) || dog[0].IntersectWith(downWall) || dog[0].IntersectWith(rightWall))
                {
                    board.StopTimer();
                    board.AddLabel(lblEnd);
                }
            }
            timerCounter++;
        }
        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (timerCounter > START + 1)
            {
                if (key == (char)ConsoleKey.LeftArrow && dog[0].direction != GameObject.Direction.RIGHT)
                    dog[0].direction = GameObject.Direction.LEFT;
                if (key == (char)ConsoleKey.RightArrow && dog[0].direction != GameObject.Direction.LEFT)
                    dog[0].direction = GameObject.Direction.RIGHT;
                if (key == (char)ConsoleKey.UpArrow && dog[0].direction != GameObject.Direction.DOWN)
                    dog[0].direction = GameObject.Direction.UP;
                if (key == (char)ConsoleKey.DownArrow && dog[0].direction != GameObject.Direction.UP)
                    dog[0].direction = GameObject.Direction.DOWN;
                if (key == (char)ConsoleKey.Spacebar && start)
                {
                    start = false;
                    board.StartTimer(timerInterval);
                    board.RemoveLabel(lblStart);
                }
                else
                {
                    if (key == (char)ConsoleKey.Spacebar && !isPaused)
                    {
                        board.StopTimer();
                        Position labelPausePosition = new Position(10, 400);
                        lblPause = new TextLabel("Paused, press spacebar to unpause", labelPausePosition);
                        lblPause.SetFont("Ariel", 14);
                        board.AddLabel(lblPause);
                        isPaused = true;
                    }
                    else
                    {
                        if (key == (char)ConsoleKey.Spacebar && isPaused)
                        {
                            board.StartTimer(timerInterval);
                            board.RemoveLabel(lblPause);
                            isPaused = false;
                        }
                    }
                }
            }
        }
        //פעולה הבודקת פגיעה עצמית
        static bool SelfHit(Board board, GameObject[] dog)
        {
            for (int i = 1; i < dog.Length; i++)
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
            int x = rnd.Next(1, 13);
            int y = rnd.Next(1, 18);
            Position pos = new Position(SIZE * x, SIZE * y);
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
            newDog[dog.Length] = new GameObject(dog[dog.Length - 1].GetPosition(), SIZE, SIZE);
            newDog[dog.Length].SetImage(Properties.Resources.dogBody);
            newDog[dog.Length].direction = dog[dog.Length - 1].direction;
            board.AddGameObject(newDog[dog.Length]);
            return newDog;
        }
        //פעולה הבודקת עם המיקום של האוכל מתנגש עם הכלב
        static bool CheckPos(GameObject[] dog, GameObject food)
        {
            for (int i = 0; i < dog.Length; i++)
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