using System.Drawing.Drawing2D;
using System.Media;
using System.Diagnostics;

namespace Test
{
    public partial class Form1 : Form
    {
        private int step;
        private string yourTankDirection;
        private int[][] arrOfPixels;
        private char[][] map;
        private int[][] arrOfTanksPositions;
        private int[][] enemyTanksDirections;
        private int[][] arrOfPossibleMoves;
        private Image[] arrOfImages;
        private bool isPossibleToShot = true;
        SoundPlayer shotSound;
        int movingSpeed = 300;
        bool[] deadTanks = new bool[4];
        Stopwatch stopWatch;
        bool[] isPossibleToShotForTheEnemyTanks = new bool[4];
        int[] times = new int[4];
        int[] arrOfTimeIntervals = new int[4];
        int heath = 3;
        bool changeHeathBar = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void FillArrOfTanksPositions()
        {
            int count = GetAmoutOfTanks();
            arrOfTanksPositions = new int[count + 1][];
            arrOfTanksPositions[0] = GetYourTankPosition();
            int step = 1;
            for (int i = 0; i < map.Length; i++)
            {
                for (int k = 0; k < map[i].Length; k++)
                {
                    if (Char.IsDigit(map[i][k]))
                    {
                        arrOfTanksPositions[step] = new int[]{ this.Height / map.Length * i,
                         (this.Width - this.Height) / 2 + this.Height / map.Length * k };
                        step++;
                    }
                }
            }
        }

        private int[] GetYourTankPosition()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'X')
                    {
                        return new int[]{ this.Height / map.Length * i,
                         (this.Width - this.Height) / 2 + this.Height / map.Length * j };
                    }
                }
            }
            return new int[0];
        }

        private int GetAmoutOfTanks()
        {
            int count = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int k = 0; k < map[i].Length; k++)
                {
                    if (Char.IsDigit(map[i][k])) count++;
                }
            }
            return count;
        }

        private void GetArrayOfTextures()
        {
            arrOfImages = new Image[15];
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\wall.png")))
                arrOfImages[0] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\greenTank.png")))
                arrOfImages[1] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\grayTank.png")))
                arrOfImages[2] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\grayTankDown.png")))
                arrOfImages[3] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\grayTankLeft.png")))
                arrOfImages[4] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\grayTankRight.png")))
                arrOfImages[5] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\road.png")))
                arrOfImages[6] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\bullet.png")))
                arrOfImages[7] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\greenTankDown.png")))
                arrOfImages[8] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\greenTankLeft.png")))
                arrOfImages[9] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\greenTankRight.png")))
                arrOfImages[10] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\fullHealth.png")))
                arrOfImages[11] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\halfHealth.png")))
                arrOfImages[12] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\lowHealth.png")))
                arrOfImages[13] = Image.FromStream(ms);
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + @"\textures\noHealth.png")))
                arrOfImages[14] = Image.FromStream(ms);
        }
        
        private void CreateArrayForMap()
        {
            map = new char[18][];
            map[0] = new String('#', 18).ToCharArray();
            map[1] = "###   #      6####".ToCharArray();
            map[2] = "###   #        ###".ToCharArray();
            map[3] = "###   #        ###".ToCharArray();
            map[4] = "#5    #   ########".ToCharArray();
            map[5] = "#     #    #     #".ToCharArray();
            map[6] = "#     #    #     #".ToCharArray();
            map[7] = "#          #     #".ToCharArray();
            map[8] = "#          #     #".ToCharArray();
            map[9] = "#####      #     #".ToCharArray();
            map[10] = "#       #       4#".ToCharArray();
            map[11] = "#       #    #####".ToCharArray();
            map[12] = "#       #    #####".ToCharArray();
            map[13] = "#       ###  #####".ToCharArray();
            map[14] = "#X      ###  #####".ToCharArray();
            map[15] = "######3 ###  #####".ToCharArray();
            map[16] = "######  ###  #####".ToCharArray();
            map[17] = new String('#', 18).ToCharArray();
        }

        private void CreateArrayOfPixels()
        {
            arrOfPixels = new int[this.Height][];
            for (int i = 0; i < arrOfPixels.Length; i++)
            {
                arrOfPixels[i] = new int[this.Height];
            }
        }

        private void GetArrOfPixels(char[][] map)
        {
            int stepI;
            int stepJ;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == ' ')
                    {
                        stepI = arrOfPixels.Length / map.Length * i;
                        stepJ = arrOfPixels.Length / map[0].Length * j;
                        for (int k = stepI; k < stepI + arrOfPixels.Length / map.Length; k++)
                        {
                            for (int l = stepJ; l < stepJ + arrOfPixels[0].Length / map.Length; l++)
                            {
                                arrOfPixels[k][l] = 1;
                            }
                        }
                    }
                    else if (map[i][j] == 'X')
                    {
                        stepI = arrOfPixels.Length / map.Length * i;
                        stepJ = arrOfPixels.Length / map[0].Length * j;
                        for (int k = stepI; k < stepI + arrOfPixels.Length / map.Length; k++)
                        {
                            for (int l = stepJ; l < stepJ + arrOfPixels[0].Length / map.Length; l++)
                            {
                                arrOfPixels[k][l] = 2;
                            }
                        }
                    }
                    else
                    {
                        for (int p = 3; p < 7; p++)
                        {
                            if (map[i][j] == p){
                                stepI = arrOfPixels.Length / map.Length * i;
                                stepJ = arrOfPixels.Length / map[0].Length * j;
                                for (int k = stepI; k < stepI + arrOfPixels.Length / map.Length; k++)
                                {
                                    for (int l = stepJ; l < stepJ + arrOfPixels[0].Length / map.Length; l++)
                                    {
                                        arrOfPixels[k][l] = p;
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }
        }    

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.ToLower(e.KeyChar) == 'w')
            {
                yourTankDirection = "north";
                MakeYourTankMove();
            }
            else if (char.ToLower(e.KeyChar) == 's')
            {
                yourTankDirection = "south";
                MakeYourTankMove();
            }
            else if (char.ToLower(e.KeyChar) == 'a')
            {
                yourTankDirection = "west";
                MakeYourTankMove();
            }
            else if (char.ToLower(e.KeyChar) == 'd')
            {
                yourTankDirection = "east";
                MakeYourTankMove();
            }
            else if (e.KeyChar == ' ')
            {
                if (isPossibleToShot)
                {
                    MakeAShot(arrOfImages[7], 2);
                }
            }
        }

        private void MakeAShotForTheEnemyTank(int tankNumber)
        {
            Graphics g = this.CreateGraphics();
            Rectangle rect = new Rectangle();
            Image img = arrOfImages[7];
            int[] tempDirection = new int[2];
            int firstBulletPosX = arrOfTanksPositions[0][0];
            int firstBulletPosY = arrOfTanksPositions[0][1];
            int tankPos1 = arrOfTanksPositions[tankNumber - 2][0];
            int tankPos2 = arrOfTanksPositions[tankNumber - 2][1];
            int tempStep = 0;

            if(enemyTanksDirections[tankNumber - 3] == arrOfPossibleMoves[0])
            {
                if (IsPossibleToMoveForBullets(firstBulletPosX - step, firstBulletPosY))
                {
                    tempDirection = new int[] { -step, 0 };
                    tempStep = -step;
                }
            }
            else if(enemyTanksDirections[tankNumber - 3] == arrOfPossibleMoves[1])
            {
                if (IsPossibleToMoveForBullets(firstBulletPosX + step, firstBulletPosY))
                {
                    tempDirection = new int[] { step, 0 };
                    tempStep = step;
                }
            }
            else if(enemyTanksDirections[tankNumber - 3] == arrOfPossibleMoves[2])
            {
                if (IsPossibleToMoveForBullets(firstBulletPosX, firstBulletPosY - step))
                {
                    tempDirection = new int[] { 0, -step };
                    tempStep = -step;
                }
            }
            else if(enemyTanksDirections[tankNumber - 3] == arrOfPossibleMoves[3])
            {
                if (IsPossibleToMoveForBullets(firstBulletPosX, firstBulletPosY + step))
                {
                    tempDirection = new int[] { 0, step };
                    tempStep = step;
                }
            }
            rect = new Rectangle(tankPos2 + tempDirection[1], tankPos1 + tempDirection[0],
                        this.Height / map.Length, this.Height / map.Length);
            g.DrawImage(img, rect);
            while (IsPossibleToMove(tempDirection[0], tempDirection[1], tankNumber))
            {
                if (tempDirection[0] != 0)
                {
                    tempDirection[0] += tempStep;
                }
                if (tempDirection[1] != 0)
                {
                    tempDirection[1] += tempStep;
                }
                rect = new Rectangle(tankPos2 + tempDirection[1], tankPos1 + tempDirection[0],
                        this.Height / map.Length, this.Height / map.Length);
                g.DrawImage(img, rect);
            }
            if (CheckIfYourTankIsShot(tempDirection[0], tempDirection[1], tankNumber, out int shotTankNumber))
            {
                MessageBox.Show("You lost one heart");
                changeHeathBar = true;
                heath--;
                if (IsPossibleToRevive());
                else {
                    GameEnd();
                    RemoveYourTank(shotTankNumber);
                }
            }
        }

        public void MakeAShot(Image img, int tankNumber)
        {
            MakeAShotSound();
            Graphics g = this.CreateGraphics();
            Rectangle rect;
            int[] tempDirection = new int[2];
            int firstBulletPosX = arrOfTanksPositions[0][0];
            int firstBulletPosY = arrOfTanksPositions[0][1];
            int tankPos1 = arrOfTanksPositions[tankNumber - 2][0];
            int tankPos2 = arrOfTanksPositions[tankNumber - 2][1];
            int tempStep = 0;
            
            if (tankNumber == 2)
            {
                isPossibleToShot = false;
                switch (yourTankDirection)
                {
                    case "north":
                        if (IsPossibleToMoveForBullets(firstBulletPosX - step, firstBulletPosY))
                        {
                            tempDirection = new int[] { -step, 0 };
                            tempStep = -step;
                        }
                        break;
                    case "south":
                        if (IsPossibleToMoveForBullets(firstBulletPosX + step, firstBulletPosY))
                        {
                            tempDirection = new int[] { step, 0 };
                            tempStep = step;
                        }
                        break;
                    case "west":
                        if (IsPossibleToMoveForBullets(firstBulletPosX, firstBulletPosY - step))
                        {
                            tempDirection = new int[] { 0, -step };
                            tempStep = -step;
                        }
                        break;
                    case "east":
                        if (IsPossibleToMoveForBullets(firstBulletPosX, firstBulletPosY + step))
                        {
                            tempDirection = new int[] { 0, step };
                            tempStep = step;
                        }
                        break;
                }
                rect = new Rectangle(tankPos2 + tempDirection[1], tankPos1 + tempDirection[0],
                            this.Height / map.Length, this.Height / map.Length);
                g.DrawImage(img, rect);
                while (IsPossibleToMove(tempDirection[0], tempDirection[1], tankNumber))
                {
                    if (tempDirection[0] != 0)
                    {
                        tempDirection[0] += tempStep;
                    }
                    if (tempDirection[1] != 0)
                    {
                        tempDirection[1] += tempStep;
                    }
                    rect = new Rectangle(tankPos2 + tempDirection[1], tankPos1 + tempDirection[0],
                            this.Height / map.Length, this.Height / map.Length);
                    g.DrawImage(img, rect);
                }
                if (CheckIfTheEnemyTankIsShot(tempDirection[0], tempDirection[1], tankNumber, out int shotTankNumber))
                {
                    RemoveTheEnemyTank(shotTankNumber);
                }
                isPossibleToShot = true;
            }           
        }

        public void RemoveYourTank(int shotTankNumber)
        {
            for (int k = 0; k < this.Height / map.Length; k++)
            {
                for (int j = 0; j < this.Height / map.Length; j++)
                {
                    arrOfPixels[arrOfTanksPositions[shotTankNumber - 2][0] + k][arrOfTanksPositions[shotTankNumber - 2][1] + j] = 1;
                }
            }
        }

        private void RemoveTheEnemyTank(int shotTankNumber)
        {
            for (int i = 0; i < 4; i++)
            {
                if (shotTankNumber == i + 2)
                {
                    deadTanks[shotTankNumber - 3] = true;
                    for (int k = 0; k < this.Height / map.Length; k++)
                    {
                        for (int j = 0; j < this.Height / map.Length; j++)
                        {
                            arrOfPixels[arrOfTanksPositions[shotTankNumber - 2][0] + k][arrOfTanksPositions[shotTankNumber - 2][1] + j] = 1;
                        }
                    }
                }
            }
        }

        private bool CheckIfYourTankIsShot(int firstCoord, int secondCoord, int tankNumber, out int shotTankNumber)
        {
            if (arrOfPixels[arrOfTanksPositions[tankNumber - 2][0]
                        + firstCoord][arrOfTanksPositions[tankNumber - 2][1] + secondCoord] == 2)
            {
                shotTankNumber = 2;
                if (tankNumber == 2) RemoveTheBrokenTank();
                return true;
            }
            shotTankNumber = 0;
            return false;
        }

        private  bool CheckIfTheEnemyTankIsShot(int firstCoord, int secondCoord, int tankNumber, out int shotTankNumber)
        {
            for (int i = 3; i < 7; i++)
            {
                if (arrOfPixels[arrOfTanksPositions[tankNumber - 2][0]
                    + firstCoord][arrOfTanksPositions[tankNumber - 2][1] + secondCoord] == i)
                {
                    shotTankNumber = i;
                    if (i == 6) RemoveTheBrokenTank();
                    return true;
                }
            }
            shotTankNumber = 0;
            return false;
        }

        private void RemoveTheBrokenTank()
        {
            int firstPos = arrOfTanksPositions[4][0];
            int secondPos = arrOfTanksPositions[4][1];
            deadTanks[3] = true;
            arrOfPixels[firstPos][secondPos] = 1;
        }

        private void MakeYourTankMove()
        {
            switch (yourTankDirection)
            {
                case "north":
                    if (IsPossibleToMove(-step, 0, 2))
                    {
                        ChangeTankPosition(-step, 0 , 2);
                    }
                    else
                    {
                        DrawAnEmptyElement(arrOfImages[6], 2);
                        ChangeTankPosition(0, 0, 2);
                    }
                    break;
                case "south":
                    if (IsPossibleToMove(step, 0, 2))
                    {
                        ChangeTankPosition(step, 0, 2);
                    }
                    else
                    {
                        DrawAnEmptyElement(arrOfImages[6], 2);
                        ChangeTankPosition(0, 0, 2);
                    }
                    break;
                case "west":
                    if (IsPossibleToMove(0, -step, 2))
                    {
                        ChangeTankPosition(0, -step, 2);
                    }
                    else
                    {
                        DrawAnEmptyElement(arrOfImages[6], 2);
                        ChangeTankPosition(0, 0, 2);
                    }
                    break;
                case "east":
                    if (IsPossibleToMove(0, step, 2))
                    {
                        ChangeTankPosition(0, step, 2);
                    }
                    else
                    {
                        DrawAnEmptyElement(arrOfImages[6], 2);
                        ChangeTankPosition(0, 0, 2);
                    }
                    break;
            }
        }
         
        private void DrawAnEmptyElement(Image img, int tankNumber)
        {
            Graphics g = this.CreateGraphics();
            Rectangle rect = new Rectangle(arrOfTanksPositions[tankNumber - 2][1], arrOfTanksPositions[tankNumber - 2][0], this.Height / map.Length, this.Height / map.Length);
            g.DrawImage(new Bitmap(img), rect);
        }

        private void ChangeTankPosition(int firstCoord, int secondCoord, int tankNumber)
        {
            for (int i = 0; i < this.Height / map.Length; i++)
            {
                for (int j = 0; j < this.Height / map.Length; j++)
                {
                    arrOfPixels[arrOfTanksPositions[tankNumber - 2][0] + i][arrOfTanksPositions[tankNumber - 2][1] + j] = 1;
                }
            }


            arrOfTanksPositions[tankNumber - 2][0] += firstCoord;
            arrOfTanksPositions[tankNumber - 2][1] += secondCoord;

            for (int i = 0; i < this.Height / map.Length; i++)
            {
                for (int j = 0; j < this.Height / map.Length; j++)
                {
                    arrOfPixels[arrOfTanksPositions[tankNumber - 2][0] + i][arrOfTanksPositions[tankNumber - 2][1] + j] = tankNumber;
                }
            }
        }

        private bool IsPossibleToMove(int firstCoord, int secondCoord, int tankNumber)
        {
            if (arrOfPixels[arrOfTanksPositions[tankNumber - 2][0] + firstCoord][arrOfTanksPositions[tankNumber - 2][1] + secondCoord] == 1)
            {
                return true;
            }
            return false;
        }

        private bool IsPossibleToMoveForBullets(int firstCoord, int secondCoord)
        {
            if (arrOfPixels[firstCoord][secondCoord] == 1)
            {
                return true;
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*PlayTheMainMusicTheme();*/
            GetArrayOfTextures();
            CreateArrayForMap();
            CreateArrayOfPixels();
            GetArrOfPixels(map);
            arrOfPixels = Upscale();
            FillArrOfTanksPositions();
            FillArrayOfEnemyTanksDirections();
            FillArrayOfPossibleEnemyTanksMoves();
            DrawTanksAtTheFirstTime();
            timer2.Start();
            timer3.Interval = movingSpeed;
            timer3.Start();
            timer1.Stop();
        }

        private void DrawTanksAtTheFirstTime()
        {
            for (int i = 2; i < arrOfTanksPositions.Length + 2; i++)
            {
                DrawAnEmptyElement(arrOfImages[6], i);
                if(i == 2) DrawAnEmptyElement(arrOfImages[1], i);
                else DrawAnEmptyElement(arrOfImages[2], i);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*this.TopMost = true;*/
            Array.Fill(isPossibleToShotForTheEnemyTanks, true);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            step = this.Height / 18;
            stopWatch = new Stopwatch();
            LoadSounds();
            stopWatch.Start();
            timer4.Start();
            timer1.Start();
        }

        private void LoadSounds()
        {
            shotSound = new SoundPlayer(Environment.CurrentDirectory + @"\sounds\shot.wav");
        }

        private void RenderTheGame()
        {
            Graphics g = this.CreateGraphics();
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            SolidBrush brush;
            Pen pen;
            Rectangle rect;
            for (int i = 0; i < arrOfPixels.Length; i += this.Height / map.Length)
            {
                for (int j = 0; j < arrOfPixels[i].Length; j += this.Height/ map.Length)
                {
                    if (arrOfPixels[i][j] == 1)
                    {
                        rect = new Rectangle(j, i, this.Height / map.Length, this.Height / map.Length);
                        g.DrawImage(new Bitmap(arrOfImages[6]), rect);
                    }
                    else if (arrOfPixels[i][j] == 2)
                    {
                        rect = new Rectangle(j, i, this.Height / map.Length, this.Height / map.Length);
                        switch (yourTankDirection)
                        {
                            case "north":
                                g.DrawImage(arrOfImages[1], rect);
                                break;
                            case "south":
                                g.DrawImage(arrOfImages[8], rect);
                                break;
                            case "west":
                                g.DrawImage(arrOfImages[9], rect);
                                break;
                            case "east":
                                g.DrawImage(arrOfImages[10], rect);
                                break;
                        }                        
                    }
                    else
                    {
                        for (int p = 3; p < arrOfTanksPositions.Length + 2; p++)
                        {
                            Image img = arrOfImages[2];
                            if (arrOfPixels[i][j] == p)
                            {
                                rect = new Rectangle(j, i, this.Height / map.Length, this.Height / map.Length);
                                if (enemyTanksDirections[p - 3] == arrOfPossibleMoves[0])
                                {
                                    g.DrawImage(arrOfImages[2], rect);
                                }
                                else if(enemyTanksDirections[p - 3] == arrOfPossibleMoves[1])
                                {
                                    g.DrawImage(arrOfImages[3], rect);
                                }
                                else if(enemyTanksDirections[p - 3] == arrOfPossibleMoves[2])
                                {
                                    g.DrawImage(arrOfImages[4], rect);
                                }
                                else if (enemyTanksDirections[p - 3] == arrOfPossibleMoves[3])
                                {
                                    g.DrawImage(arrOfImages[5], rect);
                                }
                            }
                        }
                    }
                }
            }

            int firstCoord = (this.Width - this.Height) / 2;
            int secondCoord = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == '#')
                    {
                        rect = new Rectangle(firstCoord, secondCoord, this.Height / map.Length, this.Height / map.Length);
                        g.DrawImage(new Bitmap(arrOfImages[0]), rect);
                    }
                    firstCoord += Convert.ToInt32(this.Height / map.Length);
                }
                secondCoord += Convert.ToInt32(this.Height / map.Length);
                firstCoord = (this.Width - this.Height) / 2;
            }
            
        }

        private int[][] Upscale()
        {
            int[][] newArr = new int[this.Height][];
            for (int i = 0; i < newArr.Length; i++)
            {
                newArr[i] = new int[this.Width];
            }
            for (int i = 0; i < arrOfPixels.Length; i++)
            {   
                for (int l = 0; l < arrOfPixels[i].Length; l++)
                {
                    if (arrOfPixels[i][l] == 1)
                    {
                        newArr[i][l + (this.Width - this.Height) / 2] = 1;
                    }
                    else if (arrOfPixels[i][l] == 2)
                    {
                        newArr[i][l + (this.Width - this.Height) / 2] = 2;
                    }
                }
            }
            return newArr;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en"));
            RenderTheGame();
            if(changeHeathBar) RenderHeath();
        }

        private void MakeAMoveForTheEnemyTank(int tankNumber)
        {
            int step = 0;
            int currentFirstDirection = enemyTanksDirections[tankNumber - 3][0];
            int currentSecondDirection = enemyTanksDirections[tankNumber - 3][1];
            
            if (IsPossibleToMove(currentFirstDirection, currentSecondDirection, tankNumber) )
            {
                if(!deadTanks[tankNumber - 3]) ChangeTankPosition(currentFirstDirection, currentSecondDirection, tankNumber);
            }
            else
            {
                while(true)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(0, 4);
                    step++;
                    if (step == 6) break;
                    if(IsPossibleToMove(arrOfPossibleMoves[index][0], arrOfPossibleMoves[index][1], tankNumber))
                    {
                        if(!deadTanks[tankNumber - 3])
                        {
                            enemyTanksDirections[tankNumber - 3] = arrOfPossibleMoves[index];
                            ChangeTankPosition(arrOfPossibleMoves[index][0], arrOfPossibleMoves[index][1], tankNumber);
                            break;
                        }
                    }
                }
            }
        }

        private void FillArrayOfEnemyTanksDirections()
        {
            enemyTanksDirections = new int[arrOfTanksPositions.Length - 1][];
            for (int i = 0; i < enemyTanksDirections.Length; i++)
            {
                enemyTanksDirections[i] = new int[] { step, 0 };
            }
        }

        private void FillArrayOfPossibleEnemyTanksMoves()
        {
            arrOfPossibleMoves = new int[4][];
            arrOfPossibleMoves[0] = new int[] { -step, 0 };
            arrOfPossibleMoves[1] = new int[] { step, 0 };
            arrOfPossibleMoves[2] = new int[] { 0, -step };
            arrOfPossibleMoves[3] = new int[] { 0, step };
        }

        private void MakeAShotSound()
        {
            shotSound.Play();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            ChangeEnemyTanksPositions();
            TanksShoting();
        }

        private void ChangeEnemyTanksPositions()
        {
            for (int i = 3; i < 7; i++)
            {
                MakeAMoveForTheEnemyTank(i);
            }
        }

        private void TanksShoting()
        {
            int timeInterval;
            Random rnd = new Random();
            for (int i = 3; i < 7; i++)
            {
                if (isPossibleToShotForTheEnemyTanks[i - 3] && !deadTanks[i-3])
                {
                    arrOfTimeIntervals[i - 3] = rnd.Next(4, 8);
                    isPossibleToShotForTheEnemyTanks[i - 3] = false;
                    MakeAShotForTheEnemyTank(i);
                }
            }
        }

        private void Victory()
        {
            stopWatch.Stop();
            timer4.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            string message = "Your time\n" + elapsedTime;
            DialogResult result = MessageBox.Show(message, "You won!!!", MessageBoxButtons.OK);
            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (!deadTanks.Contains(false))
            {
                Victory();
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            times = times.Select(x => x += 1).ToArray();
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] == arrOfTimeIntervals[i])
                {
                    isPossibleToShotForTheEnemyTanks[i] = true;
                    times[i] = 0;
                }
            }
        }

        private bool IsPossibleToRevive()
        {
            if (heath != 0) return true;
            else return false;
        }

        private void Revive()
        {
            int stepI;
            int stepJ;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'X')
                    {
                        stepI = arrOfPixels.Length / map.Length * i;
                        stepJ = arrOfPixels.Length / map[0].Length * j;
                        for (int k = stepI; k < stepI + arrOfPixels.Length / map.Length; k++)
                        {
                            for (int l = stepJ; l < stepJ + arrOfPixels[0].Length / map.Length; l++)
                            {
                                arrOfPixels[k][l] = 2;
                            }
                        }
                    }
                }
            }
        }

        private void GameEnd()
        {
            DialogResult result = MessageBox.Show("Try again later.", "You lost!!!", MessageBoxButtons.OK);
            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void RenderHeath()
        {
            changeHeathBar = false;
            Graphics g = this.CreateGraphics();
            int heigth = this.Height / 2 - 51/2;
            
            Rectangle rect = new Rectangle(100, heigth, 126, 51);
            switch (heath)
            {
                case 3:
                    g.DrawImage(arrOfImages[11], rect);
                    break;
                case 2:
                    g.DrawImage(arrOfImages[12], rect);
                    break;
                case 1:
                    g.DrawImage(arrOfImages[13], rect);
                    break;
                case 0:
                    g.DrawImage(arrOfImages[14], rect);
                    break;
            }
        }
    }

    public class Movement
    {

    }

    public class Shoting
    {

    }

    public class Input
    {

    }
}