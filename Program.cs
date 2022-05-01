using System;
using System.Diagnostics;
using System.IO;

namespace minesweeper
{
    
    public class square{      

        private int xLocation{get; set;}
        private int yLocation{get;set;}
        private string boxType{get;set;}        
        private bool mine{get;set;}
        private bool flag{get;set;}
        private int adjacent{get;set;}
        private bool revealed{get;set;}

        public bool getRevealed(){
            return revealed;
        }

        public void setRevealed(bool rev){
            this.revealed = rev;
        }
        
        public void setFlag(bool flag){
            this.flag = flag;
        }

        public bool getFlag(){
            return flag;
        }

        public bool getMine(){
            return mine;
        }   

        public string getBoxType(){
            return boxType;
        }
        public int getAdjacent(){
            return adjacent;
        }
        public void setAdjacent(int adj){
            this.adjacent = adj;
        }

        public square(int x, int y, string box , bool rev, bool flag){
            this.xLocation = x;
            this.yLocation = y;
            this.boxType = box;
            this.revealed = rev;           
        }
        public void addMine(){
            this.mine = true;
        }       

        public void setBoxType(string box){
            this.boxType = box;
        }

        public int getXLocation(){
            return xLocation;
        }
        public int getYLocation(){
            return yLocation;
        }        
    }
    public class MinesweeperBoard{
        public int moves = 0;
        public const int boardSize = 8;
        public const int numberOfMines = 10;
        public const int minBoardLocation = 0;

        public int count = numberOfMines;

        private square[,] board = new square[boardSize,boardSize];        

        public square[,] createBoard(){                       
            for(int i = 0; i<boardSize ;i++){
                for(int j = 0; j<boardSize; j++){
                    board[i,j] = new square(i,j,"[ ]",false,false);                    
                }
            }
            createMines(board);
            createAdjacent(board);
            return board;
        } 

        //randomly creates mines based on the numberOfMines set in the program
        public void createMines(square[,] board){
            Random rnd = new Random();
            int mines = numberOfMines; 
            while(mines > 0){
                int i = rnd.Next(boardSize);
                int j = rnd.Next(boardSize);
                if(board[i,j].getMine() == false){
                    board[i,j].addMine();                    
                    mines--;
                }
            }
        }
        //code to create the numbers assigned to each square if a mine is adjacent
        public void createAdjacent(square[,] board){
            for(int i = 0; i<boardSize;i++){
                for(int j= 0; j<boardSize;j++){
                    /*find each mine on the board and add 1 to each adjacent square if it is within the gameboard
                    each if statement corresponds to one of the 8 square that are possibly around the mine
                    the if statements are set up to stop outofbounds exceptions*/
                    if(board[i,j].getMine() == true){
                        //top left
                        if((i-1>=minBoardLocation) && (j-1>= minBoardLocation)){                            
                            board[i-1,j-1].setAdjacent(board[i-1,j-1].getAdjacent()+1);
                        }
                        //middle left
                        if((i-1>=minBoardLocation)){                            
                            board[i-1,j].setAdjacent(board[i-1,j].getAdjacent()+1);
                        }
                        //top middle
                        if((j-1>= minBoardLocation)){                            
                            board[i,j-1].setAdjacent(board[i,j-1].getAdjacent()+1);
                        }
                        //bottom right
                        if((i+1<boardSize)&&(j+1)<boardSize){
                            board[i+1,j+1].setAdjacent(board[i+1,j+1].getAdjacent()+1);
                        }
                        //middle right
                        if((i+1<boardSize)){
                            board[i+1,j].setAdjacent(board[i+1,j].getAdjacent()+1);
                        }
                        //bottom middle
                        if((j+1<boardSize)){
                            board[i,j+1].setAdjacent(board[i,j+1].getAdjacent()+1);
                        }
                        //bottom left
                        if((i-1>=minBoardLocation)&&(j+1<boardSize)){
                            board[i-1,j+1].setAdjacent(board[i-1,j+1].getAdjacent()+1);
                        }
                        //top right
                        if((j-1>=minBoardLocation)&&(i+1<boardSize)){
                            board[i+1,j-1].setAdjacent(board[i+1,j-1].getAdjacent()+1);
                        }                        
                    }
                }
            }
        }

        public void revealSquare(int x,int y){
            board[x,y].setRevealed(true);
            board[x,y].setBoxType("["+board[x,y].getAdjacent()+"]");
            if(board[x,y].getFlag()==true){
                board[x,y].setFlag(false);                
            }            
        }
        public void openAllAround(int x,int y){
            //takes in x,y and opens itself and the 8 spaces around it if they exist in the board
            if(board[x,y].getAdjacent() == 0){
                revealSquare(x,y);
                if((x-1>=minBoardLocation) && (y-1>= minBoardLocation)){                    
                    revealSquare(x-1,y-1);                                 
                }
                if((x-1>=minBoardLocation) && (y+1<boardSize)){
                    revealSquare(x-1,y+1);                                    
                }
                if((x+1<boardSize) && (y+1<boardSize)){                    
                    revealSquare(x+1,y+1);                                  
                }
                if((x+1<boardSize)&& (y-1>=minBoardLocation)){
                    revealSquare(x+1,y-1);
                }
                if(x+1<boardSize){
                    revealSquare(x+1,y);
                }
                if(x-1>=minBoardLocation){
                    revealSquare(x-1,y);
                }
                if(y+1<boardSize){
                    revealSquare(x,y+1);
                }
                if(y-1>=minBoardLocation){
                    revealSquare(x,y-1);
                }
            }
            else{
                revealSquare(x,y);
            }
        }
        //loops through each square in the board finding all revealed 0s and then revealing every square around them
        public void openAll(int x,int y){
            //found that two passes one going from [0,0] to [boardsize-1,boardsize-1] then passing through going the reverse revealed most things
            //two passes of openAll reveals all I've tested. Probably a better way to do this thats accurate all the time with less time complexity 
            openAllAround(x,y);
            for(int i = 0;i<boardSize;i++){
                for(int j = 0;j<boardSize;j++){
                    if(board[i,j].getRevealed()==true){
                        openAllAround(i,j);
                    }
                }
            }
            for(int i = boardSize - 1; i >= 0; i--){
                for(int j = boardSize - 1; j >= 0; j--){
                    if(board[i,j].getRevealed()==true){
                        openAllAround(i,j);
                    }
                }
            }                      
        }
        public bool checkWin(){
            int numberOfSquares = boardSize * boardSize;
            int numberRevealed = 0;

            //check each square and count the number of revealed squares
            for(int i=0;i<boardSize;i++){
                for(int j=0;j<boardSize;j++){
                    if(board[i,j].getRevealed()==true){
                        numberRevealed++;
                    }
                }
            }
            //numberRevealed + numberOfMines == numberOfSquares is a way to check that all squares are revealed but mines meaning you've won the game
            if(numberRevealed + numberOfMines == numberOfSquares){                
                return true;
            }
            else{
                return false;
            }

        }


        //used to show all the mines when you lose
        public void showMines(){
            for(int i=0;i<boardSize;i++){
                for(int j=0;j<boardSize;j++){
                    if(board[i,j].getMine()==true){
                        board[i,j].setBoxType("[x]");
                    }
                }
            }
        }

        //used to show how many mines are remaining relative to the number of flags you've created
        public void checkFlags(){
            int flags = 0;
            for(int i=0;i<boardSize;i++){
                for(int j=0;j<boardSize;j++){
                    if(board[i,j].getFlag()==true){
                        flags++;
                    }
                }
            }
            Console.WriteLine("Possible mines remaining "+ (count-flags));
        }

        //move counter to save high scores lower will be better
        public void increaseMoves(){
            moves++;
            Console.WriteLine("Current number of moves: "+moves);
        }
        
        //takes in the user input to choose a col row and action. Depending on the input different actions are carried out returns a bool used to determin if the game is over

        public bool chooseSquare(string input){
            
            bool lost = false;
            //takes the char at input[1] - the ascii value for char '0' into an int -1 to keep the proper index
            int col = Convert.ToInt32(input[1] - '0')-1;
            //takes the char at input[1]'s ascii value - the ascii value for the char 'a' and converts to an int
            int row = Convert.ToInt32(input[0] - 'a');            
            char action = Convert.ToChar(input[2]);

            //make sure the input is within the board
            if(col>=boardSize || row>=boardSize){
                Console.WriteLine("Your input is out of bounds try again");
                return lost;
            }
            //pick the square will reveal it and if its a 0 will keep revealing until no adjacent squares are 0           
            else if(action == 'p'){
                if(board[row,col].getMine() == false){                    
                    openAll(row,col);
                    openAll(row,col);                                                         
                    return lost;
                }
                //if a mine is picked return that the game is lost and show all the mines on the board                
                else{
                    showMines();
                    lost = true;
                    Console.WriteLine("Game over. You hit a mine better luck next time");
                    return lost;                    
                }
            }
            //used to mark the square and set a flag
            else if(action == 'm'){
                board[row,col].setBoxType("[M]");
                board[row,col].setFlag(true);               
                return lost;
            }
            //used if action isnt p or m
            else{
                Console.WriteLine("Incorrect action selected try again");
                return lost;
            }
        }

        //used to print the board with the correct indicators across the top and side
        public void printBoard(){
            string[] side = {"a ","b ","c ","d ","e ","f ","g ","h "};
            string[] top ={"1","2","3","4","5","6","7","8"};
            Console.Write("  ");
            for(int i = 0; i<top.Length;i++){
                Console.Write("  "+top[i]+" ");
            }
            Console.WriteLine();
            for(int i= 0; i<boardSize;i++){
                Console.Write(side[i]+" ");
                for(int j = 0; j<boardSize; j++){                    
                    Console.Write(board[i,j].getBoxType()+ " ");
                }
                Console.WriteLine();
            }
        }
    }
    class Player{
        int numberOfMoves{get;set;}
        string name{get;set;}
        double time{get;set;}

        public int getNumberOfMoves(){
            return this.numberOfMoves;
        }
        public void setNumberOfMoves(int moves){
            this.numberOfMoves = moves;
        }
        public string getName(){
            return this.name;
        }
        public void setName(){
            Console.Write("Enter your name: ");            
            this.name = Console.ReadLine();
        }
        public double getTime(){
            return this.time;
        }
        public void setTime(long timeMS){
            this.time = timeMS/1000;
        }
    }
    class Program
    {
        static string getInput(){
            Console.WriteLine("Available actions are p for pick and m for mark");
            Console.Write("Enter row column and action (ex: to mark a1 type a1m) : ");
            string input = Console.ReadLine().Trim();
            if(input.Length == 3){            
                return input;
            }
            else{
                Console.WriteLine("\n incorrect input length defaulted to a1m");
                return "a1m";
            }
        }       
        public static bool startGame(MinesweeperBoard board){
            bool win = true;
            bool loss = false;            
            board.createBoard();            
            board.printBoard();
            
            while(board.chooseSquare(getInput()) == false && !board.checkWin()){
                board.printBoard();
                board.checkFlags();
                board.increaseMoves();                
            }
            if (board.checkWin()){
                Console.WriteLine("Congratualtions you've won!");
                return win;
            }
            else{
                return loss;
            }
        }
        public static void writeSave(int moves, string name, double time, bool winLoss, int mines){
            string path = @"players.txt";
            if(winLoss){
                if(!File.Exists(path)){
                    using(StreamWriter streamWriter = File.CreateText(path)){
                        streamWriter.WriteLine(name);
                        streamWriter.WriteLine("Number of moves: "+moves);
                        streamWriter.WriteLine("Finished in: "+ time +" seconds");
                        streamWriter.WriteLine("Number of mines: "+ mines);
                    }
                }
                else{
                    using(StreamWriter streamWriter = File.AppendText(path)){
                        streamWriter.WriteLine();
                        streamWriter.WriteLine(name);
                        streamWriter.WriteLine("Number of moves: "+moves);
                        streamWriter.WriteLine("Finished in: "+ time +" seconds");
                        streamWriter.WriteLine("Number of mines: "+ mines);
                    }
                }
            }
        }
        static void Main(string[] args)
        {   
            Console.WriteLine("Previous Wins!");
            string highScores = File.ReadAllText(@"players.txt");
            Console.WriteLine(highScores);   

            Player player = new Player();
            player.setName(); 
            Stopwatch watch = new Stopwatch();
            MinesweeperBoard board = new MinesweeperBoard();
            watch.Start();
            bool winLoss = startGame(board);
            board.printBoard();
            player.setTime(watch.ElapsedMilliseconds);
            player.setNumberOfMoves(board.moves);
            watch.Stop();
            writeSave(player.getNumberOfMoves(),player.getName(),player.getTime(), winLoss, MinesweeperBoard.numberOfMines);            

        }
    }
}
