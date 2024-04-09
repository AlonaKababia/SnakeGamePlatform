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
using static SnakeGamePlatform.GameObject;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace SnakeGamePlatform
{
    public class GameEvents : IGameEvents
    {
        #region variables
        //Define game variables here! for example...
        //GameObject [] snake;
        TextLabel lblScore; TextLabel lblEnd; TextLabel lblPause; TextLabel lblStartCredit; TextLabel lblDifficulty; TextLabel lblStart; TextLabel lblPresent; TextLabel lblNameOfGame1; TextLabel lblNameOfGame2; TextLabel lblGoodMsg; TextLabel lblSuper;
        GameObject food; GameObject paw;
        int points = 0, countEaten = 0;
        GameObject[] dog = new GameObject[2];
        GameObject upWall, leftWall, downWall, rightWall;
        const int BONUS = 5;
        const int SIZE = 40;
        int timerInterval = 200;
        bool isPaused = false, start = true, gameOver = false;
        int timerCounter = 0, current = 0;
        const int START = 20, MID = 160, HARD = 110;
        string difficulty = "difficulty: easy";
        const string FontNotForTitle = "Aharoni";
        Direction dirImg = Direction.RIGHT;
        #endregion
        //This function is called by the game one time on initialization!
        //Here you should define game board resolution and size (x,y).
        //Here you should initialize all variables defined above and create all visual objects on screen.
        //You could also start game background music here.
        //use board Object to add game objects to the game board, play background music, set interval, etc...
        public void GameInit(Board board)
        {
            #region GameObjects settings
            //Setup board size and resolution!
            Board.resolutionFactor = 1;
            board.XSize = 600;
            board.YSize = 800;
            //Adding a text label to the game board.
            //Name of game lbl first part
            Position NameOfGamePos1 = new Position(300, 240);
            lblNameOfGame1 = new TextLabel("Pa", NameOfGamePos1);
            lblNameOfGame1.SetFont("Britannic bold", 27);
            //Name of game the paw image
            Position NameOfGamePawPos = new Position(300, 295);
            paw = new GameObject(NameOfGamePawPos, 35, 40);
            paw.SetImage(Properties.Resources.paw);
            //Name of game lbl second part
            Position NameOfGamePos2 = new Position(300, 330);
            lblNameOfGame2 = new TextLabel("s in the palace", NameOfGamePos2);
            lblNameOfGame2.SetFont("Britannic bold", 27);
            //names lbl
            Position creditPos = new Position(200, 60);
            lblStartCredit = new TextLabel("Yonatan Volsky, Itamar Goffer, Alona Kababia and Noa Avitov", creditPos);
            lblStartCredit.SetFont(FontNotForTitle, 16);
            board.AddLabel(lblStartCredit);
            //PRESENTS lbl
            Position lblPresentPos = new Position(260, 270);
            lblPresent = new TextLabel("PRESENT:", lblPresentPos);
            lblPresent.SetFont(FontNotForTitle, 25);
            board.AddLabel(lblPresent);
            //score lbl
            Position labelPosition = new Position(10, 10);
            lblScore = new TextLabel($"Score: {points.ToString()}", labelPosition);
            lblScore.SetFont(FontNotForTitle, 14);
            //game over lbl
            Position labelEndPosition = new Position(260, 210);
            lblEnd = new TextLabel("Game Over! Press R to reset", labelEndPosition);
            lblEnd.SetFont(FontNotForTitle, 25);
            //difficulty lbl
            Position labelDifPos = new Position(10, 120);
            lblDifficulty = new TextLabel(difficulty, labelDifPos);
            lblDifficulty.SetFont(FontNotForTitle, 14);
            //pause lbl
            Position labelPausePosition = new Position(10, 300);
            lblPause = new TextLabel("Paused, press spacebar to npause", labelPausePosition);
            lblPause.SetFont(FontNotForTitle, 14);
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
            Position dogPosition = new Position(200, 120);
            dog[0] = new GameObject(dogPosition, SIZE, SIZE);
            dog[0].SetImage(Properties.Resources.dogPixile);
            dog[0].direction = GameObject.Direction.RIGHT;
            //dogBody
            Position dogBodyPosition = new Position(200, 80);
            dog[1] = new GameObject(dogBodyPosition, SIZE, SIZE);
            dog[1].SetImage(Properties.Resources.bulldogPixile);
            dog[1].direction = GameObject.Direction.RIGHT;
            //food
            Position foodPosition = new Position(200, 480);
            food = new GameObject(foodPosition, SIZE, SIZE);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            #endregion
            //Start game timer!
            board.StartTimer(timerInterval);
            //מוזיקת מסך פתיחה
            board.PlayBackgroundMusic(@"\Images\pauseMusic.mp4");
            //Background color credits
            board.SetBackgroundColor(Color.Crimson);
        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            if (timerCounter == START + 1)
            {
                #region setting up the board
                board.RemoveLabel(lblStartCredit); // after credit screen, removing and adding labels&game objects
                board.RemoveLabel(lblPresent);
                board.RemoveLabel(lblEnd);
                board.SetBackgroundColor(Color.White);
                board.AddLabel(lblScore);
                board.AddLabel(lblDifficulty);
                board.AddGameObject(upWall); board.AddGameObject(leftWall); board.AddGameObject(downWall); board.AddGameObject(rightWall);
                board.AddGameObject(dog[0]);
                board.AddGameObject(dog[1]);
                board.AddGameObject(food);
                //Play file in loop!
                board.PlayBackgroundMusic(@"\Images\syria.wav");
                //Play file once!
                board.SetBackgroundImage(Properties.Resources.BuckinghamPalace);
                #endregion
            }
            if (timerCounter > START)
            {
                if (current + 3 == timerCounter) { board.RemoveLabel(lblGoodMsg); } // erasing the good msg
                if (start)
                {
                    #region starting screen
                    board.StopTimer();
                    board.AddLabel(lblNameOfGame1);
                    board.AddGameObject(paw);
                    board.AddLabel(lblNameOfGame2);
                    Position startGameSentence = new Position(10, 400);
                    lblStart = new TextLabel("Press the space bar of the game to start", startGameSentence);
                    lblStart.SetFont(FontNotForTitle, 14);
                    board.AddLabel(lblStart);
                    #endregion
                }
                else
                {
                    board.RemoveLabel(lblNameOfGame1);
                    board.RemoveGameObject(paw);
                    board.RemoveLabel(lblNameOfGame2);
                    #region eating
                    //eating
                    bool isEating = dog[0].IntersectWith(food);
                    if (isEating)
                    {
                        if (timerCounter - current < 3) //check for fast eating
                        {
                            board.RemoveLabel(lblGoodMsg);
                            lblGoodMsg = GoodMsg(board, dog[0]);
                            current = timerCounter;
                        }
                        else
                        {
                            lblGoodMsg = GoodMsg(board, dog[0]);
                            current = timerCounter;
                        }
                        if (countEaten % BONUS == 0 && countEaten != 0)
                        { // if goes in here, special food!
                            points += 30;
                            board.RemoveLabel(lblSuper);
                            board.PlayShortMusic(@"\Images\superFoodSound.wav");
                            while (!CheckPos(dog, food))
                            {
                                food.SetPosition(RandomFoodPos(dog));
                            }
                            food.SetImage(Properties.Resources.food);
                            dog = AddBody(dog, board);
                            countEaten++;
                        }
                        else
                        { // normal food
                            points += 10;
                            //להשמיע קול אכילה
                            board.PlayShortMusic(@"\Images\eatingMusic.wav");
                            while (!CheckPos(dog, food) || !food.OnScreen(board))
                            {
                                food.SetPosition(RandomFoodPos(dog));
                            }
                            if (countEaten % BONUS == BONUS - 1)
                            {
                                board.RemoveLabel(lblGoodMsg);
                                Random superMsg = new Random();
                                int imgNum = superMsg.Next(1, 4);
                                lblSuper = SuperImgAndMsg(board, imgNum, food);
                            }
                            dog = AddBody(dog, board);
                            countEaten++;
                        }
                        lblScore.SetText($"Score: {points.ToString()}");
                        #endregion
                        #region difficulty
                        if (timerInterval > 75) // adding difficulty
                        {
                            timerInterval -= 5;
                            board.StartTimer(timerInterval);
                            if (timerInterval < MID) // changing difficulty label
                            {
                                difficulty = "difficulty: medium";
                                lblDifficulty.SetText(difficulty);
                            }
                            if (timerInterval < HARD)
                            {
                                difficulty = "difficulty: hard";
                                lblDifficulty.SetText(difficulty);
                            }
                        }
                        #endregion
                    }
                    #region movement
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
                        SetImg(dirImg, dog[0]);
                        dirImg = Direction.RIGHT;
                    }
                    if (dog[0].direction == GameObject.Direction.LEFT)
                    {
                        dogPosition.Y = dogPosition.Y - SIZE;
                        dog[0].SetPosition(dogPosition);
                        SetImg(dirImg, dog[0]);
                        dirImg = Direction.LEFT;
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
                    #endregion
                    #region Game Over
                    //game over check
                    if (SelfHit(board, dog) || dog[0].IntersectWith(upWall) || dog[0].IntersectWith(leftWall) || dog[0].IntersectWith(downWall) || dog[0].IntersectWith(rightWall))
                    {
                        board.PlayShortMusic(@"\Images\gameOverSound.wav");
                        board.StopTimer();
                        for (int i = 0; i < dog.Length; i++)
                        {
                            board.RemoveGameObject(dog[i]);
                        }
                        board.RemoveGameObject(food);
                        board.RemoveLabel(lblGoodMsg);
                        if (countEaten % BONUS == 0 && countEaten != 0) { board.RemoveLabel(lblSuper); }
                        board.AddLabel(lblEnd);
                        food.SetImage(Properties.Resources.food);
                        countEaten = 0;
                        board.PlayBackgroundMusic(@"\Images\pauseMusic.mp4");
                        timerInterval = 200;
                        gameOver = true;

                    }
                    #endregion
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
                #region movement keys
                if (key == (char)ConsoleKey.LeftArrow && dog[0].direction != GameObject.Direction.RIGHT)
                    dog[0].direction = GameObject.Direction.LEFT;
                if (key == (char)ConsoleKey.RightArrow && dog[0].direction != GameObject.Direction.LEFT)
                    dog[0].direction = GameObject.Direction.RIGHT;
                if (key == (char)ConsoleKey.UpArrow && dog[0].direction != GameObject.Direction.DOWN)
                    dog[0].direction = GameObject.Direction.UP;
                if (key == (char)ConsoleKey.DownArrow && dog[0].direction != GameObject.Direction.UP)
                    dog[0].direction = GameObject.Direction.DOWN;
                #endregion
                #region Start/Pause (spacebar)
                if (key == (char)ConsoleKey.Spacebar && start)
                {
                    start = false;
                    board.StartTimer(timerInterval);
                    board.RemoveLabel(lblStart);
                }
                else
                {
                    if (key == (char)ConsoleKey.Spacebar && !isPaused && !gameOver)
                    {
                        board.StopTimer();
                        board.AddLabel(lblPause);
                        board.PlayBackgroundMusic(@"\Images\pauseMusic.mp4");
                        isPaused = true;
                    }
                    else
                    {
                        if (key == (char)ConsoleKey.Spacebar && isPaused)
                        {
                            board.StartTimer(timerInterval);
                            board.RemoveLabel(lblPause);
                            board.PlayBackgroundMusic(@"\Images\syria.wav");
                            isPaused = false;
                        }
                    }
                    #endregion
                    #region reset
                    if (key == (char)ConsoleKey.R && gameOver)
                    {
                        timerInterval += countEaten * 5;
                        board.StartTimer(timerInterval);
                        countEaten = 0;
                        timerCounter = START + 1;
                        dog = ResetDog(dog);
                        //reseting evetything!
                        //dog head
                        Position dogPosition = new Position(200, 120);
                        dog[0] = new GameObject(dogPosition, SIZE, SIZE);
                        dog[0].SetImage(Properties.Resources.dogPixile);
                        dog[0].direction = GameObject.Direction.RIGHT;
                        //dogBody
                        Position dogBodyPosition = new Position(200, 80);
                        dog[1] = new GameObject(dogBodyPosition, SIZE, SIZE);
                        dog[1].SetImage(Properties.Resources.bulldogPixile);
                        dog[1].direction = GameObject.Direction.RIGHT;
                        //food
                        Position foodPosition = new Position(200, 480);
                        food = new GameObject(foodPosition, SIZE, SIZE);
                        food.SetImage(Properties.Resources.food);
                        food.direction = GameObject.Direction.RIGHT;
                        points = 0; lblScore.SetText($"Score: {points.ToString()}");
                        difficulty = "difficulty: easy"; lblDifficulty.SetText(difficulty);
                        start = true;
                        gameOver = false;
                        dirImg = Direction.RIGHT;
                        SetImg(dirImg, dog[0]);
                    }
                }
                #endregion
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
            newDog[dog.Length].SetImage(Properties.Resources.bulldogPixile);
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
        //פעולה היוצרת את ה"הודעה הטובה" ומחזירה אותה
        static TextLabel GoodMsg(Board board, GameObject dogHead)
        {
            Random r = new Random();
            int x = r.Next(1, 4);
            string msg = "";
            if (x == 1) { msg = "Good Job!"; }
            if (x == 2) { msg = "Excellent!"; }
            if (x == 3) { msg = "Amazing!"; }
            Position msgPos = new Position(10, 630);
            TextLabel lblGoodMsg = new TextLabel(msg, msgPos);
            lblGoodMsg.SetFont(FontNotForTitle, 14);
            board.AddLabel(lblGoodMsg);
            return lblGoodMsg;
        }
        //פעולה שבודקת מה המספר של הסופר ומתאימה את התמנה וההודעה לפיו
        static TextLabel SuperImgAndMsg(Board board, int img, GameObject food)
        {
            string imgMsg = "";
            if (img == 1)
            {
                food.SetImage(Properties.Resources.superFood);
                imgMsg = "Eat the bicuit!";
            }
            if (img == 2)
            {
                food.SetImage(Properties.Resources.bus);
                imgMsg = "Catch the bus!!";
            }
            if (img == 3)
            {
                food.SetImage(Properties.Resources.crown);
                imgMsg = "Can you steal the queen's crown??";
            }
            Position msgPos = new Position(10, 350);
            TextLabel superMsg = new TextLabel(imgMsg, msgPos);
            superMsg.SetFont("Ariel", 14);
            board.AddLabel(superMsg);
            return superMsg;
        }
        //פעולה המאתחלת את המערך של הכלב
        static GameObject[] ResetDog(GameObject[] dog)
        {
            GameObject[] newDog = new GameObject[2];
            newDog[0] = dog[0];
            newDog[1] = dog[1];
            return newDog;
        }
        //rotating a GameObject Image 180 degrees horizontaly
        static void RotateImg(GameObject obj)
        {
            Image img = obj.GetImage();
            img.RotateFlip(RotateFlipType.Rotate180FlipY);
            obj.SetImage(img);
        }
        //פעולה המקבלת עצם וכיוון התמונה שלו, ומתאים בניהם כשצריך. הפעולה מחזירה את הכיוון החדש
        static Direction SetImg(Direction dir, GameObject obj)
        {
            if (obj.direction == Direction.RIGHT)
            {
                if (dir == Direction.LEFT)
                {
                    RotateImg(obj);
                }
                return obj.direction;
            }
            else
            {
                if (obj.direction == Direction.LEFT)
                {
                    if (dir == Direction.RIGHT)
                    {
                        RotateImg(obj);
                    }
                    return obj.direction;
                }
            }
            return dir;
        }
    }
}