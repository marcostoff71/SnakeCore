using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _079_JoinString
{
    public enum Direccion
    {
        Arriba,
        Abajo,
        Derecha,
        Izquierda,
        Ninguno
    }
    public class Snake
    {
        
        private int delay;
        private int puntos;
        private Random rdn;
        private int alto;

        private int ancho;

        private bool terminado;

        private Direccion direActual;

        private List<Point> snake;

        private Point comidaSnake;

        public event EventHandler GameOver;
        public event EventHandler EGano;
        public event EventHandler<int> Comio;


        public Snake(int alto = 10,int ancho=20)
        {
            this.rdn = new Random();
            this.alto = alto;
            this.ancho = ancho;
            this.snake = new List<Point>();
            Reset();   
        }
        private void SimulaGanador()
        {
            while (snake.Count < (alto - 2) * (ancho - 2))
            {
                snake.Add(new Point(rdn.Next(1, alto), rdn.Next(1, alto)));
            }
        }
        private void Reset()
        {
            this.delay = 300;
            this.puntos = 0;
            this.terminado = false;
            this.direActual = Direccion.Derecha;
            this.snake.Clear();
            InicializaSnake(snake);
            this.comidaSnake = GeneraComida();

        }
        private void InicializaSnake(List<Point> snake)
        {
            int x = (ancho / 2) - 2;
            int y = alto / 2;
            for(int i = 0; i < 3; i++)
            {
                snake.Insert(0, new Point(x + i, y));
            }
            
        }
        private Point GeneraComida()
        {

            Point aux = new Point();
            bool esCorre=false;
            do
            {

                aux.X = this.rdn.Next(1, ancho-1);
                aux.Y = this.rdn.Next(1, alto-1);

                esCorre=!snake.Any(x=> x.X == aux.X && x.Y == aux.Y);
            } while (!esCorre);

            return aux;
        }
        private Direccion PreguntaDirec()
        {
            Direccion dire = Direccion.Ninguno;
            if (Console.KeyAvailable)
            {
                ConsoleKey key=Console.ReadKey().Key;


                if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    dire = Direccion.Arriba;
                }else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    dire = Direccion.Abajo;
                }else if(key == ConsoleKey.RightArrow || key == ConsoleKey.D)
                {
                    dire = Direccion.Derecha;
                }else if(key == ConsoleKey.LeftArrow || key == ConsoleKey.A)
                {
                    dire = Direccion.Izquierda;
                }
            }

            return dire;
        }
        public void MuestraSnakeEnConsola()
        {
            void Marco()
            {
                for (int i = 0; i < ancho; i++)
                {


                    //Console.SetCursorPosition(left, top);

                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(i, 0);
                    Console.Write("*");

                    Console.SetCursorPosition(i, alto - 1);
                    Console.Write("*");

                }
                for (int i = 0; i < alto; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(0, i);
                    Console.Write("*");
                    Console.SetCursorPosition(ancho - 1, i);
                    Console.Write("*");
                }

                Console.ResetColor();
            }
            
            snake.ForEach(e =>
            {
                Console.SetCursorPosition(e.X, e.Y);
                if (e == snake.First())
                {
                    
                    
                    Console.Write('*');
                }
                else
                {
                   
                    Console.Write("*");
                }
            });

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(comidaSnake.X, comidaSnake.Y);
            Console.Write("x");

            Console.ResetColor();
            Console.SetCursorPosition(0, alto);
            
            Console.Write($"Puntos: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(puntos);
            Console.ResetColor();

            Marco();

        }
        private void MueveSnake()
        {
            Direccion aux = PreguntaDirec();
            Point pPrimero = snake.First();
            if (aux != Direccion.Ninguno)
            {

                if(aux == Direccion.Arriba &&direActual!=Direccion.Abajo)
                {
                    this.direActual = aux;
                    
                }else if(aux==Direccion.Abajo && direActual!= Direccion.Arriba)
                {
                    this.direActual = aux;

                }else if (aux == Direccion.Derecha && direActual != Direccion.Izquierda)
                {
                    this.direActual = aux;

                }else if(aux==Direccion.Izquierda && direActual != Direccion.Derecha)
                {
                    this.direActual = aux;
                }




            }

            if (direActual == Direccion.Arriba)
            {
                pPrimero.Y--;
            }else if (direActual == Direccion.Abajo)
            {
                pPrimero.Y++;
            }else if(direActual == Direccion.Derecha)
            {
                pPrimero.X++;
            }else if(direActual == Direccion.Izquierda)
            {
                pPrimero.X--;
            }

            snake.Insert(0, pPrimero);

            if (AgarroComida())
            {

                OnComio();
                this.delay -= 10;
                this.puntos++;
                if(!GanoSnake())this.comidaSnake = GeneraComida();

            }
            else
            {

                snake.RemoveAt(snake.Count - 1);
            }

        }
        private bool Perdio()
        {
            Point point = snake.First();

            bool perdio = false;
            if (point.X <= 0 || point.X >= ancho-1)
            {
                perdio = true;
            }else if(point.Y<=0 || point.Y >= alto-1)
            {
                perdio = true;
            }
            else
            {
               perdio=snake.Skip(1).Any(e=>e.X==point.X&&e.Y==point.Y);

            }

            return perdio;
        }
        private bool GanoSnake()
        {
            return snake.Count ==( (alto - 2) * (ancho - 2));
        }
        private bool AgarroComida()
        {

            Point primero=snake.First();
            
            return primero.X==comidaSnake.X && comidaSnake.Y==primero.Y;
        }
        public void InitGame()
        {

            while (!terminado)
            {

                MueveSnake();

                //SimulaGanador();
                if (Perdio()||GanoSnake())
                {

                    if (Perdio())
                    {
                        OnGameOver();
                    }
                    else
                    {
                        OnWinner();
                    }
                    break;
                }
                else
                {
                    Console.Clear();

                }
               
                MuestraSnakeEnConsola();
                Thread.Sleep(delay);
            }
        }
        private void OnWinner()
        {
            this.EGano?.Invoke(this, new EventArgs());
        }
        private void OnComio()
        {
            this.Comio?.Invoke(this, puntos);
        }
        private void OnGameOver()
        {
            this.GameOver?.Invoke(this, new EventArgs());
        }

        
        

    }
}
