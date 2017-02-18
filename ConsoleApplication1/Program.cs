using System;

namespace ConsoleApplication1
{
    class Program
    {
        // Values declared in 01.40
        static double altitude;     //altitude
        static double velocity;     //vessel
        static double mass;         //vessel mass
        static double fuelmass;     //fuel mass

        static double tensec;       //time
        static double q;
        static double i;
        static double j;
        static double burn;         //Burn qty
        static double elapsed = 0;  //elapsed time
        static double step;         //step
        static double velMPH;       //vertical speed
        
        static string outcome;      //result message

        const double z = 1.800;
        const double gravity = 0.001;   //gravity

        static void Main(string[] args)
        {
            //Window Setup
            Console.SetWindowSize(70, 25);
            Console.SetBufferSize(70, 25);
            Console.Title = "LUNAR - Jim Storer, Ted Thompson";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();

            //Print header on the screen.
            centerPrint("LUNAR");
            centerPrint("Originally written for the DEC PDP-8 by Jim Storer, 1969");
            centerPrint("Ported to C# by Ted Thompson, 2017");
            Console.WriteLine("======================================================================");
            Console.WriteLine("");
            Console.WriteLine("");

            // 01.04
            //Start Port of Original Program
            Console.WriteLine("CONTROL CALLING LUNAR MODULE. MANUAL CONTROL IS NECESSARY");
            Console.WriteLine("YOU MAY RESET FUEL RATE K EACH 10 SECS TO 0 OR ANY VALUE");
            Console.WriteLine("BETWEEN 8 & 200 LBS/SEC. YOU'VE {0:F0} LBS FUEL. ESTIMATED", mass - fuelmass);
            Console.WriteLine("FREE FALL IMPACT TIME-120 SECS. CAPSULE WEIGHT-{0:F0} LBS", mass);
            GroupOne();
        }

        static void GroupOne()
        {
            Console.WriteLine("FIRST RADAR CHECK COMING UP");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("COMMENCE LANDING PROCEDURE");
            Console.WriteLine("TIME,SECS   ALTITUDE,MILES+FEET   VELOCITY,MPH   FUEL,LBS   FUEL RATE");

            //Init Variables 04.10 (G[ravity] and Z are constants and defined above)
            altitude = 120;
            velocity = 1;
            mass = 32500;
            fuelmass = 16500;            
  
            while (true) //GroupTwo
            {
                do
                {
                    // Elapsed Time
                    Console.Write("{0,8:F0}", elapsed);
                    // Altitude
                    Console.Write("{0,15}{1,7}", Math.Floor(altitude), Math.Floor(5280 * (altitude - Math.Floor(altitude))));
                    // VSI
                    Console.Write("{0,15:F2}", 3600 * velocity);
                    // Fuel
                    Console.Write("{0,12:F1}", mass - fuelmass);
                    // Burn Setting Prompt
                    Console.Write("{0,9}", "K=:");
                    do
                    {
                        try
                        {
                            string input = Console.ReadLine();
                            if (input == "")
                            {
                                burn = -1;
                            }
                            burn = Double.Parse(input);
                        }
                        catch (FormatException)
                        {
                            ThreePointSevenTwo();
                            burn = -1;
                        }
                        //Console.Write(burn);
                    } while (burn == -1); // parse loop

                    //TODO add checks for less then 8 more then 200

                    tensec = 10;
                    GroupThree();
                } while (true);  //this loop 150-170
            } //end of endless-while loop
        }
// 01.50-02.10
        static void GroupThree()
        {
            while (true)
            {
                if (mass - fuelmass < .001) //3.10
                {
                    GroupFour();
                }
                if (tensec < .001)
                {
                    return;
                }
                step = tensec; // 03.10 end statement
                if (fuelmass + step * burn - mass <= 0)
                {
                    GroupNine();
                }
                else
                {
                    step = (mass - fuelmass) / burn;
                    GroupNine();
                }
                if (i <= 0)
                {
                    GroupSeven();
                }
                if (velocity <= 0)
                {
                    continue;
                }
                if (j < 0)
                {
                    GroupEight();
                    continue;
                }
                GroupSix();
                //return;
            }
        }
// 03.80
// 04.10
        static void GroupFour()
        {
            Console.WriteLine("FUEL OUT AT " + elapsed + " SECS");
            //s = ((v * -1) + Math.Sqrt(v * v + 2 * altitude * gravity)) / gravity;
            step = (Math.Sqrt(velocity * velocity + 2 * altitude * gravity) - velocity) / gravity;
            velocity = velocity + gravity * step;
            elapsed = elapsed + step;
            touchDown();
        }
// 04.40
// 05.10
        static void touchDown() //260
        {
            velMPH = 3600 * velocity;
            Console.WriteLine("ON THE MOON AT {0,8:F2} SECS", elapsed);
            Console.WriteLine("IMPACT VELOCITY OF {0,8:F2} M.P.H.", velMPH);
            Console.WriteLine("FUEL LEFT:{0,9:F2} LBS", mass - fuelmass);
            outcome = "CRAFT DAMAGED... YOU'RE STRANDED HERE UNTIL A RESCUE\nPARTY ARRIVES. HOPE YOU HAVE ENOUGH OXYGEN!\n";
            if (velMPH > 60)
            {
                outcome = string.Format ("SORRY,BUT THERE WERE NO SURVIVORS-YOU BLEW IT!\nIN FACT YOU BLASTED A NEW LUNAR CRATER{0,9:F2} FT. DEEP\n", velMPH * .277);
            }
            if (velMPH <= 10)
            {
                outcome = "GOOD LANDING (COULD BE BETTER)\n";
            }
            if (velMPH <= 1.2)
            {
                outcome = "PERFECT LANDING!\n";
            }
            Console.WriteLine(outcome);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("TRY AGAIN?");
            while (true)
            {
                Console.Write("(ANS. YES OR NO)");
                string p = Console.ReadLine();
                if (p.ToUpper() == "NO")
                {
                    Console.WriteLine("CONTROL OUT");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    System.Threading.Thread.Sleep(1000);
                    Environment.Exit(1);
                }
                else if (p.ToUpper() == "YES")
                {
                    GroupOne();
                }
            }
        }
// 05.98
// 06.10
        static void GroupSix()  
        {
            elapsed = elapsed + step;
            tensec = tensec - step;
            mass = mass - step * burn;
            altitude = i;
            velocity = j;
            return;
        }                       
// 06.10
// 07.10
        static void GroupSeven()
        {
            while (!(step < .005))
            {
                step = 2 * altitude / (velocity + Math.Sqrt(velocity * velocity + 2 * altitude * (gravity - z * burn / mass)));
                GroupNine();
                GroupSix();
            }
            touchDown();
        }                       
// 07.30
// 08.10
        static void GroupEight()    
        {
            do
            {
                velMPH = (1 - mass * gravity / z * burn) / 2;
                step = mass * velocity / (z * burn * (velMPH + Math.Sqrt(velMPH * velMPH + velocity / z))) + .05;
                GroupNine();
                if (i <= 0)
                {
                    GroupSeven();
                }
                GroupSix();
                if (j >= 0)
                {
                    return;
                }
            } while (!(velocity > 0));
            return;
        }                           
// 08.30
// 09.10
        static void GroupNine()   
        {
            double q2 = Math.Pow(q, 2);
            double q3 = Math.Pow(q, 3);
            double q4 = Math.Pow(q, 4);
            double q5 = Math.Pow(q, 5);
            double neg_q = q * -1;

            q = step * burn / mass;
            j = velocity + gravity * step + z * (neg_q - q2 / 2 - q3 / 3 - q4 / 4 - q5 / 5);
            i = altitude - gravity * step * step / 2 - velocity * step + z * step * (q / 2 + q2 / 6 + q3 / 12 + q4 / 20 + q5 / 30);

            return;
        }                       
// 09.40
// Routine by me for centering header text.
        static void centerPrint(string msg)
        {
            Console.WriteLine("{0," + (Console.WindowWidth + msg.Length) / 2 + "}", msg);
        }

        static void ThreePointSevenTwo()
        {
            Console.Write("NOT POSSIBLE");
            for(i=1; i < 52; i++)
            {
                Console.Write(".");
            }
            Console.Write("K=:");
            return;
        }
    }
}

//Console.WriteLine("---------1---------2---------3----^----4---------5---------6---------7");

/****************************************************************
 * Original Program
 * 
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
02.20 T %6.02,"       "3600*V,"    "%6.01,M-N,"      K=";A K;S T=10
02.70 T %7.02;I (200-K)2.72;I (8-K)3.1,3.1;I (K)2.72,3.1
02.72 T "NOT POSSIBLE";F X=1,51;T "."
02.73 T "K=";A K;G 2.7

03.10 I (M-N-.001)4.1;I (T-.001)2.1;S S=T
03.40 I (N+S*K-M)3.5,3.5;S S=(M-N)/K
03.50 D 9;I (I)7.1,7.1;I (V)3.8,3.8,I (J)8.1
03.80 D 6;G 3.1

04.10 T "FUEL OUT AT"L," SECS"!
O4.40 S S=(FSQT(V*V+2*A*G)-V)/G;S V=V+G*S;S L=L+S

05.10 T "ON THE MOON AT"L," SECS"!;S w=3600*V
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

06.19 S L=L+S;S T=T-S;S M=M-S*K;S A=1;s V=J

07.10 I (S-.005)5.1;S S=2*A/(V+FSQT(V*V+2*A*(G-Z*K/M)))
07.30 D 9;D 6;G 7.1

08.10 S W=(1-M*G/Z*K)/2;S S=M*V/(Z*K*(W+FSQT(W*W+V/Z)))+.05;D 9
08.30 I (I)7.1,7.1;D 6;I (-J)3.1,3.1;I (V)3.l,3.1,8.1

09.10 S Q=S*K/M;S J=V+G*S+Z*(-Q-Q^2/2-Q^3/3-Q^4/4-Q^5/5)
09.40 S I=A-G*S*S/2-V*S+Z*S*(Q/2+q^2/6+Q^3/12+Q^4/20*Q^5/30)
*
*************************************************************************************/
