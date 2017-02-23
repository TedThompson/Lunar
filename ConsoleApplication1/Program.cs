using System;
using System.Runtime.InteropServices;
using static System.Math;

namespace Rocket // Original name
{
    class Lunar  // Name used for later versions in BASIC
    {
        static decimal altitude;        //altitude
        static decimal velocity;        //vessel speed
        static decimal mass;            //vessel mass
        static decimal tensec;          //time
        static decimal quantity;
        static decimal tmpAlt;
        static decimal tmpVel;
        static decimal burn;            //Burn qty
        static decimal elapsed = 0;     //elapsed time
        static decimal step;            //step
        static decimal velMPH;          //vertical speed
        
        static string outcome;          //result message

        const decimal netmass = 16500;  //fuel mass
        const decimal z = 1.8M;
        const decimal gravity = 0.001M; //gravity

        static bool gameover = false;   //Flag to quit game
        
        /* Below is code to simply disable the Maximize button, Minimize button, and Size
         * menu option so the window will always mimic the fixed width display (printer  
         * really) of a PDP-8 
         */
        
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_SIZE = 0xF000;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /* End of console control code */

        static void Main(string[] args)
        {
            // Window Setup
            // These DeleteMenu calls do the dirty work of nuking the unwanted buttons / options
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);

            // Set up size, titlebar, and colors
            Console.SetWindowSize(70, 25);
            Console.SetBufferSize(70, 25);
            Console.Title = "LUNAR - Jim Storer, Ted Thompson";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            // End window setup

            // Print header on the screen.
            centerPrint("LUNAR");
            centerPrint("Originally written for the DEC PDP-8 by Jim Storer, 1969");
            centerPrint("Ported to C# by Ted Thompson, 2017");
            Console.WriteLine("======================================================================\n\n");

            // 01.04
            //Start Port of Original Program
            Console.Write("CONTROL CALLING LUNAR MODULE. MANUAL CONTROL IS NECESSARY\n");
            Console.Write("YOU MAY RESET FUEL RATE K EACH 10 SECS TO 0 OR ANY VALUE\n");
            Console.Write("BETWEEN 8 & 200 LBS/SEC. YOU'VE 16000 LBS FUEL. ESTIMATED\n");
            Console.Write("FREE FALL IMPACT TIME-120 SECS. CAPSULE WEIGHT-32500 LBS\n");

            do
            {
                StartGame();
            } while (!gameover);

            Console.Write("CONTROL OUT\n\n\n*");
            System.Threading.Thread.Sleep(3000); // Give folks time to see Control Out before the window closes
            Environment.Exit(1);
        }

        static void StartGame()
        {
            // Init Variables 04.10 (G[ravity], N[etmass] and Z are constants and defined above)
            altitude = 120;
            velocity = 1;
            mass = 32500;
            elapsed = 0; // Not in original FOCAL code, but appears in later versions, 
                         // so I'm assumeing it's absense was a bug.  Without this the time keeps
                         // accumulating on subsequent play throughs.

            Console.Write("FIRST RADAR CHECK COMING UP\n\n\n");
            Console.Write("COMMENCE LANDING PROCEDURE\nTIME,SECS   ALTITUDE,");
            Console.Write("MILES+FEET   VELOCITY,MPH   FUEL,LBS   FUEL RATE\n");

            while (true)
            {
                // Elapsed Time
                Console.Write("{0,8:F0}", elapsed);
                // Altitude
                Console.Write("{0,15}{1,7}", Truncate(altitude), Truncate(5280 * (altitude - Truncate(altitude))));
                // VSI
                Console.Write("{0,15:F2}", 3600 * velocity);
                // Fuel
                Console.Write("{0,12:F1}", mass - netmass);
                // Burn Setting Prompt
                Console.Write("{0,9}", "K=:");

                do
                {
                    string input = Console.ReadLine();
                    try
                    {
                        burn = decimal.Parse(input);
                    }
                    catch (FormatException)
                    {
                        burn = (input == "") ? 0 : -1;
                    }
                    if ((burn != 0) && (burn < 8) || (burn > 200))
                    {
                        Console.Write("NOT POSSIBLE");
                        for (tmpAlt = 1; tmpAlt < 52; tmpAlt++)
                        {
                            Console.Write(".");
                        }
                        Console.Write("K=:");
                        burn = -1;
                    }
                } while (burn == -1); // parse loop

                tensec = 10;
                do
                {
                    if (mass - netmass - 0.001M < 0) // 3.10
                    {
                        AfterActionReport(true, false);
                        return;
                    }
                    if (tensec - 0.001M < 0)
                    {
                        continue;
                    }
                    step = tensec; // 03.10 end statement
                    if (netmass + step * burn - mass > 0)
                    {
                        step = (mass - netmass) / burn;
                    }
                    DecentCalc();
                    if (tmpAlt <= 0)
                    {
                        AfterActionReport(false, true);
                        return;
                    }
                    if (velocity > 0)
                    {
                        if (tmpVel < 0)
                        {
                            // GroupEight();
                            do
                            {
                                velMPH = (1 - mass * gravity / (z * burn)) / 2;
                                step = mass * velocity / (z * burn * (velMPH + (decimal)Sqrt((double)velMPH * (double)velMPH + (double)velocity / (double)z))) + .05M;
                                DecentCalc();
                                if (tmpAlt <= 0)
                                {
                                    AfterActionReport(false, true);
                                    return;
                                }
                                EndTurn();
                                if (tmpVel > 0)
                                {
                                    // GroupThree();
                                    continue;
                                }
                            } while (velocity > 0);

                            continue;
                        }
                    }
                    EndTurn();
                } while (tensec - 0.001M > 0);
            }
        }

        /// <summary>
        /// End of game report on outcome
        /// </summary>
        /// <param name="outtagas">Are we out of fuel</param>
        /// <param name="impacted">Did we descend below 0ft in the last 10 seconds</param>
        static void AfterActionReport(bool outtagas, bool impacted) //260
        {
            if (impacted)
            {
                while (step - 0.005M > 0)
                {
                    step = 2 * altitude / (velocity + (decimal)Sqrt((double)velocity * (double)velocity + 2 * (double)altitude * ((double)gravity - (double)z * (double)burn / (double)mass)));
                    DecentCalc();
                    EndTurn();
                }
            }
            if (outtagas)
            {
                Console.Write("FUEL OUT AT {0,8:F2} SECS\n", elapsed);
                step = ((decimal)Sqrt((double)velocity * (double)velocity + 2 * (double)altitude * (double)gravity) - velocity) / gravity;
                velocity = velocity + gravity * step;
                elapsed = elapsed + step;
            }
            velMPH = 3600 * velocity;
            Console.Write("ON THE MOON AT {0,8:F2} SECS\n", elapsed);
            Console.Write("IMPACT VELOCITY OF {0,8:F2} M.P.H.\nFUEL LEFT:{1,9:F2} LBS\n", velMPH, mass - netmass);

            if (velMPH <= 1)
            {
                outcome = "PERFECT LANDING !-(LUCKY)\n";
            }
            else if (velMPH <= 10)
            {
                outcome = "GOOD LANDING-(COULD BE BETTER)";
            }
            else if (velMPH <= 22)
            {
                outcome = "CONGRATULATIONS ON A POOR LANDING";
            }
            else if (velMPH < 40)
            {
                outcome = "CRAFT DAMAGE. GOOD LUCK";
            }
            else if (velMPH < 60)
            {
                outcome = "CRASH LANDING-YOU'VE 5 HRS OXYGEN";
            }
            else
            {
                outcome = string.Format("SORRY,BUT THERE WERE NO SURVIVORS-YOU BLEW IT!\nIN FACT YOU BLASTED A NEW LUNAR CRATER{0,9:F2} FT. DEEP\n", velMPH * .277777M);
            }
            Console.WriteLine(outcome);
            Console.Write("\n\n\n\nTRY AGAIN?\n");
            while (true)
            {
                Console.Write("(ANS. YES OR NO)");
                string p = Console.ReadLine();
                if (p.ToUpper() == "NO")
                {
                    gameover = true;
                    return;
                }
                else if (p.ToUpper() == "YES")
                {
                    return;
                }
            }
        }

// 06.10 (Subroutine)
        static void EndTurn()  
        {
            elapsed = elapsed + step;
            tensec = tensec - step;
            mass = mass - step * burn;
            altitude = tmpAlt;
            velocity = tmpVel;
            return;
        }                       

// 09.10 (Subroutine)
        static void DecentCalc()   
        {
            quantity = step * burn / mass;

            decimal q2 = quantity * quantity;  
            decimal q3 = quantity * quantity * quantity; 
            decimal q4 = quantity * quantity * quantity * quantity; 
            decimal q5 = quantity * quantity * quantity * quantity * quantity; 
            decimal neg_q = quantity * -1;
            tmpVel = velocity + gravity * step + z * (neg_q - q2 / 2 - q3 / 3 - q4 / 4 - q5 / 5);
            tmpAlt = altitude - gravity * step * step / 2 - velocity * step + z * step * (quantity / 2 + q2 / 6 + q3 / 12 + q4 / 20 + q5 / 30);

            return;
        }

        // Routine added for centering header text.
        /// <summary>
        /// Print msg centered in window
        /// </summary>
        /// <param name="msg">String to print</param>
        static void centerPrint(string msg)
        {
            Console.WriteLine("{0," + (Console.WindowWidth + msg.Length) / 2 + "}", msg);
        }

        // Print routine found at 02.72 of the original program to inform the player 
        // they made an invalid entry.
        static void TwoPointSevenTwo()
        {
            Console.Write("NOT POSSIBLE");
            for(int x=1; x < 52; x++)
            {
                Console.Write(".");
            }
            Console.Write("K=:");
            return;
        }
    }
}

/****************************************************************
 * Original Program                                             *
 **************************************************************** 

     W
    C-FOCAL,1969

    01.04 T "CONTROL CALLING LUNAR MODULE- MANUAL CONTROL IS NECESSARY"!
    01.06 T "YOU MAY RESET FUEL RATE K EACH 10 SECS TO 0 OR ANY VALUE"!
    01.08 T "BETWEEN 8 & 200 LBS/SEC- YOU'VE 16000 LBS FUEL. ESTIMATED"!
    01.11 T "FREE FALL IMPACT TIME-120 SECS. CAPSULE WEIGHT-32500 LBS"!
    01.20 T "FIRST RADAR CHECK COMING UP"!!!;E
    01.30 T "COMMENCE LANDING PROCEDURE"!"TIME,SECS   ALTITUDE,"
    01.40 T "MILES+FEET   VELOCITY-MPH   FUEL-LBS   FUEL RATE"!
    01.50 S A=120;S V=1;S M=32500;S N=16500;S G=.001;S Z=1.8

    02.10 T "    "%3,L,"           "FITR(A),"  "$4,5280*(A-FITR(A))
    02.20 T %6.02,"       "3600*V,"    "%6.01,M-N,"      K=";A K;S T=L0
    02.70 T %7.02;I (200-K)2.72;I (8-K)3.1,3.1;I (K)2.72,3.1
    02.72 T "NOT POSSIBLE";F X=1,51;T "."
    02.73 T "K=";A K;G 2.7

    03.10 I (M-N-.001)4.1;I (T-.001)2.1;S S=T
    03.40 I (N+S*K-M)3.5,3.5;S S=(M-N)/K
    03.50 D 9;I (I)7.1,7.1;I (V)3.8,3.8;I (J)8.1
    03.80 D 6;G 3.1

    04.10 T "FUEL OUT AT"L," SECS"!
    O4.40 S S=(FSQT(V*V+2*A*G)-V)/G;S V=V+G*S;S L=L+S

    05.10 T "ON THE MOON AT"L," SECS"!;S W=3600*V
    05.20 T "IMPACT VELOCITY OF"W," M-P-H-"!"FUEL LEFT:"M-N," LBS"!
    05.40 I (1-W)5.5,5.5;T "PERFECT LANDING !-(LUCKY)"!;G 5.9
    05.50 I (10-W)5.6,5.6;T "GOOD LANDING-(COULD BE BETTER)";G 5.9
    05.60 I (22-W)5.7,5.7;T "CONGRATULATIONS ON A POOR LANDING";G 5.9
    05.70 I (40-W)5.81,5.81;T "CRAFT DAMAGE. GOOD LUCK";G 5.9
    05.81 I (60-W)5.82,5.82;T "CRASH LANDING-YOU'VE 5 HRS OXYGEN";G 5.9
    05.82 T "SORRY,BUT THERE WERE NO SURVIVORS-YOU BLEW IT!"!"IN "
    05.83 T "FACT YOU BLASTED A NEW LUNAR CRATER"W*.277777," FT. DEEP"!
    05.90 T !!!!"TRY AGAIN?"!
    05.92 A "(ANS. YES OR NO)"P;1 (P-0NO)5.94,5.98
    05.94 I (P-0YES)5.92,1.2,5.92
    05.98 T "CONTROL OUT"!!!;Q

    06.19 S L=L+S;S T=T-S;S M=M-S*K;S A=I;S V=J

    07.10 I (S-.005)5.1;S S=2*A/(V+FSQT(V*V+2*A*(G-Z*K/M)))
    07.30 D 9;D 6;G 7.1

    08.10 S W=(1-M*G/Z*K)/2;S S=M*V/(Z*K*(W+FSQT(W*W+V/Z)))+.05;D 9
    08.30 I (I)7.1,7.1;D 6;I (-J)3.1,3.1;I (V)3.1,3.1,8.1

    09.10 S Q=S*K/M;S J=V+G*S+Z*(-Q-Q^2/2-Q^3/3-Q^4/4-Q^5/5)
    09.40 S I=A-G*S*S/2-V*S+Z*S*(Q/2+Q^2/6+Q^3/12+Q^4/20*Q^5/30)
    *

    Information on FOCAL69 can be found at the following address:
    http://homepage.divms.uiowa.edu/~jones/pdp8/focal/focal69.html#summary
//  ---------1---------2---------3----^----4---------5---------6---------7
*/
