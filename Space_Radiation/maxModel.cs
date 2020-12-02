using System;
using System.Collections.Generic;
using System.Text;


    sealed class maxModel : IDisposable
    {
        static double FKB1 = 0, FKB2 = 0, FINCR2 = 0, FINCR1 = 0;
        static double FKBM = 0, FLOGM = 0, SL2 = 0, FNB = 0, DFL = 0;
        static int J1 = 0, J2 = 0, ITIME = 0, L1 = 0, L2 = 0;
        static int I2 = 0, FLOG1 = 0, FLOG2 = 0;
        static double FKB = 0, FLOG = 0;
        static double FKBJ1 = 0, FKBJ2 = 0, SL1 = 0;
        static double FISTEP = 0;
        static int I1 = 0;
        unsafe static int[] MAP;
        static int[] MAPPRTNS;
        static int[] MAPELTNS;

        static double trara5()
        {
            double retval;
            if (FKB < (FKBM + 1e-10)) return 0;
            retval = FLOGM + (FLOG - FLOGM) * ((FNB - FKBM) / (FKB - FKBM));
            retval = retval > 0 ? retval : 0;
            return (retval);
        }

        unsafe static double trara4(int* SUBMAP, int start_psn)
        {
            bool bypassto20;
            bypassto20 = false;

            /* reset statics */
            FKB = FLOG = 0;

            if (start_psn == 15)
            {
                ;
            }
            else if (start_psn == 35)
            {
                /*    35  FINCR1=0.
                 SL1=-900000.
                 GOTO 20  */
                FINCR1 = 0;
                SL1 = -900000;
                bypassto20 = true; /* like going to 20 */
            }
            else
            {
                /* 29 FKBM=FKBJ1+(FKB2-FKBJ1)*DFL */
                FKBM = FKBJ1 + (FKB2 - FKBJ1) * DFL;
                FLOGM = FKBM * SL2;
                FLOG2 = (int)(FLOG2 - FISTEP);
                FKB2 = FKB2 + FINCR2;
                SL1 = FLOG1 / FKB1;
                SL2 = FLOG2 / FKB2;
            }
            while (true)
            {
                if (SL1 >= SL2 && !bypassto20)
                {
                    FKBJ2 = ((FLOG2 / FISTEP) * FINCR2 + FKB2) / ((FINCR2 / FISTEP) * SL1 + 1);
                    FKB = FKB1 + (FKBJ2 - FKB1) * DFL;
                    FLOG = FKB * SL1;
                    if (FKB >= FNB) return (trara5());
                    FKBM = FKB;
                    FLOGM = FLOG;
                    if (J1 >= L1) return (0);
                    J1 = J1 + 1;
                    FINCR1 = SUBMAP[I1 + J1 - 1];
                    FLOG1 = (int)(FLOG1 - FISTEP);
                    FKB1 = FKB1 + FINCR1;
                    SL1 = FLOG1 / FKB1;
                }
                bypassto20 = false; /* only once occurs via entry at 35 */
                FKBJ1 = ((FLOG1 / FISTEP) * FINCR1 + FKB1) / ((FINCR1 / FISTEP) * SL2 + 1);
                FKB = FKBJ1 + (FKB2 - FKBJ1) * DFL;
                FLOG = FKB * SL2;
                if (FKB >= FNB) return (trara5());
                FKBM = FKB;
                FLOGM = FLOG;
                if (J2 >= L2) return (0);
                J2 = J2 + 1;
                FINCR2 = SUBMAP[I2 + J2 - 1];
                FLOG2 = (int)(FLOG2 - FISTEP);
                FKB2 = FKB2 + FINCR2;
                SL2 = FLOG2 / FKB2;
            }  /* GOTO 15  (or possibly 20??) */
        } /* end of TRARA2 */

        unsafe static double trara3(int* SUBMAP, int position)
        {

            /* reset statics */
            FKBJ1 = FKBJ1 = SL1 = 0;

            if (position == 23 && (ITIME != 1) && (J2 != 4))
            {
                SL2 = FLOG2 / FKB2;
                for (J1 = 4; J1 <= L1; J1++)
                {
                    FINCR1 = SUBMAP[I1 + J1 - 1];
                    FKB1 = FKB1 + FINCR1;
                    FLOG1 = (int)(FLOG1 - FISTEP);
                    FKBJ1 = ((FLOG1 / FISTEP) * FINCR1 + FKB1) / ((FINCR1 / FISTEP) * SL2 + 1);
                    /* IF(FKBJ1.LE.FKB1) GOTO 31 */
                    if (FKBJ1 <= FKB1) return (trara4(SUBMAP, 0)); /*go to 31 with sure true;
                                           same as go to 29 or trara4(0) */
                }
                if (FKBJ1 <= FKB2) return (trara5());
                if (FKBJ1 <= FKB2) return (trara4(SUBMAP, 0)); /* start at the top of trara4() */
                FKB1 = 0;
            }
            FKB2 = 0;
            if (position == 32)
            {
                J2 = 4;
                FINCR2 = SUBMAP[I2 + J2 - 1];
                FLOG2 = SUBMAP[I2 + 3 - 1];
                FLOG1 = SUBMAP[I1 + 3 - 1];
            }
            FLOGM = FLOG1 + (FLOG2 - FLOG1) * DFL;
            FKBM = 0;
            FKB2 = FKB2 + FINCR2;
            FLOG2 = (int)(FLOG2 - FISTEP);
            SL2 = FLOG2 / FKB2;
            if (L1 < 4) return (trara4(SUBMAP, 35));
            J1 = 4;
            FINCR1 = SUBMAP[I1 + J1 + 1];
            FKB1 = FKB1 + FINCR1;
            FLOG1 = (int)(FLOG1 - FISTEP);
            SL1 = FLOG1 / FKB1;
            return (trara4(SUBMAP, 15));
        }  /* end of function trara3() */

        unsafe static double TRARA2(int* SUBMAP, double IL, double IB)
        {
            
            double FNL, FLL1, FLL2;
            int I2, KT;
            /* TRARA2 may becalled multiple times --want to reset these statics */
            I2 = FLOG1 = FLOG2 = J1 = J2 = ITIME = L1 = L2 = 0;
            FKB1 = FKB2 = FINCR2 = FINCR1 = FKBM = FLOGM = SL2 = FNB = DFL = 0;

            FNB = IB;
            FNL = IL;
            /*
             * FIND CONSECUTIVE SUB-SUB-MAPS FOR SCALED L-VALUES LS1,LS2, 
             * WITH IL LESS OR EQUAL LS2.  L1,L2 ARE LENGTHS OF SUB-SUB-MAPS. 
             * I1,I2 ARE INDECES OF FIRST ELEMENTS MINUS 1.
             */
            L2 = SUBMAP[I2 + 1 - 1];
            
            while (SUBMAP[I2 + 2 - 1] <= IL)
            {
                I1 = I2;
                L1 = L2;
                I2 = I2 + L2;
                L2 = SUBMAP[I2 + 1 - 1];
                
            }

            if ((L1 < 4) && (L2 < 4))
                return (trara5());

            if (SUBMAP[I2 + 3 - 1] <= SUBMAP[I1 + 3 - 1])
            {
                KT = I1; /* 5 KT=I1   */
                I1 = I2;
                I2 = KT;
                KT = L1;
                L1 = L2;
                L2 = KT;
            }
            do
            {   /* major 5 loop of type while is here */
                /*
                 * DETERMINE INTERPOLATE IN SCALED L-VALUE
                 */
                FLL1 = SUBMAP[I1 + 2 - 1];
                FLL2 = SUBMAP[I2 + 2 - 1];
                DFL = (FNL - FLL1) / (FLL2 - FLL1);
                FLOG1 = SUBMAP[I1 + 3 - 1];
                FLOG2 = SUBMAP[I2 + 3 - 1];
                FKB1 = 0;
                FKB2 = 0;
                if (L1 < 4) return (trara3(SUBMAP, 32));
                /*
                 * B/B0 LOOP
                 */
                for (J2 = 4; J2 <= L2; J2++)
                {
                    FINCR2 = SUBMAP[I2 + J2 - 1];
                    /* IF(FKB2+FINCR2.GT.FNB) GOTO 23 */
                    if ((FKB2 + FINCR2) > FNB) return (trara3(SUBMAP, 23));
                    FKB2 = FKB2 + FINCR2;
                    /* 17 FLOG2=FLOG2-FISTEP */
                    FLOG2 = (int)(FLOG2 - FISTEP);
                }  /* closing 17 here */
                ITIME = ITIME + 1;
                /* IF(ITIME.EQ.1)GO TO 5  */
                KT = I1;
                I1 = I2;
                I2 = KT;
                KT = L1;
                L1 = L2;
                L2 = KT;
            } while (ITIME == 1);  /* this is the 5 continue */
            return (0); /*GO TO 50, equiv to TRARA2=0 */
        } /* end of trara2() here */

        unsafe void TRARA1(double FL, double BB0, double[] E, ref double[] F, int N, int[] DESCR)
        {
            bool S0, S1, S2;
            int I2, I3, L3, IE, I0;
            double ESCALE, FSCALE;
            double XNL, NL, NB, E0, E1, E2, F0, F1, F2;
            F1 = 1.001; F2 = 1.002;

            FISTEP = DESCR[6] / DESCR[1];
            ESCALE = DESCR[3];
            FSCALE = DESCR[6];
            FL = FL > 0 ? FL : -FL;
            XNL = 15.6 < FL ? 15.6 : FL;

            NL = XNL * DESCR[4];
            if (BB0 < 1.0) BB0 = 1;
            NB = (BB0 - 1.0) * DESCR[6 - 1];
            ////////////////////////////////;
            /*                                                                       
             * I2 IS THE NUMBER OF ELEMENTS IN THE FLUX MAP FOR THE FIRST ENERGY.  
             * I3 IS THE INDEX OF THE LAST ELEMENT OF THE SECOND ENERGY MAP.       
             * L3 IS THE LENGTH OF THE MAP FOR THE THIRD ENERGY.                   
             * E1 IS THE ENERGY OF THE FIRST ENERGY MAP (UNSCALED)                 
             * E2 IS THE ENERGY OF THE SECOND ENERGY MAP (UNSCALED)                
             */
            I1 = 0;
            I0 = 0;//
            E0 = 0;//
            I2 = MAP[1 - 1];

            I3 = I2 + MAP[I2 + 1 - 1];
            L3 = MAP[I3 + 1 - 1];
            E1 = MAP[I1 + 2 - 1] / ESCALE;
            E2 = MAP[I2 + 2 - 1] / ESCALE;

            /*
             * S0, S1, S2 ARE LOGICAL VARIABLES WHICH INDICATE WHETHER THE FLUX FOR 
             * A PARTICULAR E, B, L POINT HAS ALREADY BEEN FOUND IN A PREVIOUS CALL  
             * TO FUNCTION TRARA2. IF NOT, S.. =.TRUE.
             */
            S1 = true;
            S2 = true;
            // S0=1 ;  //==================================================SO
            /* 
             *			ENERGY LOOP
             */
            /* DO 3 IE=1,N */
            for (IE = 1; IE <= N; IE++)
            {
                /*
                 * FOR EACH ENERGY E(I) FIND THE SUCCESSIVE ENERGIES E0,E1,E2 IN 
                 * MODEL MAP, WHICH OBEY  E0 < E1 < E(I) < E2 . 
                 */
                //printf("PARAMETER %f",E[IE-1]);
                while (!((E[IE - 1] <= E2) || L3 == 0))
                {
                    I0 = I1;
                    I1 = I2;
                    I2 = I3;
                    I3 = I3 + L3;
                    /* L3=MAP(I3-1) */
                    L3 = MAP[I3 + 1 - 1];
                    E0 = E1;
                    E1 = E2;
                    /* E2=MAP[I2+2]/ESCALE */
                    E2 = MAP[I2 + 2 - 1] / ESCALE;
                    S0 = S1;
                    S1 = S2;
                    /* S2=.TRUE. */
                    S2 = true;
                    F0 = F1;
                    F1 = F2;
                } /* while continues here */
                /*
                 * CALL TRARA2 TO INTERPOLATE THE FLUX-MAPS FOR E1,E2 IN L-B/B0-
                 * SPACE TO FIND FLUXES F1,F2 [IF THEY HAVE NOT ALREADY BEEN 
                 * CALCULATED FOR A PREVIOUS E(I)].
                 */
                if (S1)
                    fixed (int *p= &MAP[I1 + 3 - 1])
                    {
                        F1 = TRARA2(p, NL, NB) / FSCALE;
                    }
                if (S2)
                    fixed (int *p = &MAP[I2 + 3 - 1])
                    {
                        F2 = TRARA2(p, NL, NB) / FSCALE;
                    }
                S1 = false;
                S2 = false;
                /*
                 * FINALLY, INTERPOLATE IN ENERGY.
                 */
                F[IE - 1] = F1 + (F2 - F1) * (E[IE - 1] - E1) / (E2 - E1);
                if (F2 <= 0.0 || I1 == 0)
                {
                    /*                                                                       
                     * --------- SPECIAL INTERPOLATION ---------------------------------
                     * IF THE FLUX FOR THE SECOND ENERGY CANNOT BE FOUND (I.E. F2=0.0),
                     * AND THE ZEROTH ENERGY MAP HAS BEEN DEFINED (I.E. I1 NOT EQUAL 0), 
                     * THEN INTERPOLATE USING THE FLUX MAPS FOR THE ZEROTH AND FIRST 
                     * ENERGY AND CHOOSE THE MINIMUM OF THIS INTERPOLATIONS AND THE
                     * INTERPOLATION THAT WAS DONE WITH F2=0. 
                     */
                    /* IF(S0) F0=TRARA2(&MAP(I0+3),NL,NB)/FSCALE  */
                    S0 = false; //F0=0;E0=0;
                    if (S0)
                    {
                        fixed (int* p = &MAP[I0 + 3 - 1])
                        {
                            F0 = TRARA2(p, NL, NB) / FSCALE;
                        }
                        /* S0=.FALSE. */

                        /* F(IE)=AMIN1(F(IE),F0+(F1-F0)*(E(IE)-E0)/(E1-E0)) */
                        if (F[IE - 1] < F0 + (F1 - F0) * (E[IE - 1] - E0) / (E1 - E0))
                            F[IE - 1] = F0 + (F1 - F0) * (E[IE - 1] - E0) / (E1 - E0);
                    }
                }
            }  /* major for loop continues here */

            F[IE - 1] = F[IE - 1] > 0 ? F[IE - 1] : 0;
        } /* trara1() ends */

        static void PopulateArrays()
        {
            MAPPRTNS = readFile("ap8max.asc");
            MAPELTNS = readFile("ae8max.asc");
        }
        unsafe static int[] readFile(String fileName)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@fileName);
            String a;
            int[] temp = new int[18000];
            int time = 0;
            while ((a = file.ReadLine()) != null)
            {
                //Console.Out.WriteLine(a);
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        int j = i;
                        for (; j < i + 6 && j < a.Length; j++)
                        {
                            if (a[j] == ' ')
                            {
                                break;
                            }
                        }
                        temp[time] = Convert.ToInt32(a.Substring(i, j - i));
                        i = j - 1;
                        time++;
                    }
                }
            }
            return temp;
        }

        public double[,] getFlux(double h, double fai)
        {
            int NUMENERG = 500;
            float Lvalue, BB0;
            double[] En = new double[NUMENERG + 10];
            double[] flux = new double[NUMENERG + 10];
            float[] Ep = new float[10];
            float[] flux_p= new float[10];
            float RangeF, bottomF, xx, yy, zz, altrad, RE, BetaValueF;
            double arg;
            int mi; float mlat;
            int[] descrelns = { 8, 4, 1964, 6400, 2100, 1024, 1024, 13168 };
            int[] descrprtns = { 2, 4, 1964, 100, 2048, 2048, 1024, 16584 };
            double[] mElect = new double[NUMENERG + 10];
            double[] mProt = new double[NUMENERG + 10];

            PopulateArrays();
            h += 6371.0;
            arg = h / 6370.0;
            RE = (float)arg;
            altrad = (float)(fai * 3.14159 / 180.0);
            BetaValueF = (float)(30610.0 * Math.Sqrt(Math.Cos(1.57 - altrad) * Math.Cos(1.57 - altrad) * 3.0 + 1.0) / (RE * RE * RE));
            bottomF = RE * RE * RE * RE * RE * RE;
            bottomF = (float)(4 - (BetaValueF * BetaValueF * bottomF / (3.06e4 * 3.06e4)));
            Lvalue = (float)(3 * Math.Sqrt(RE * RE) / bottomF);
            BB0 = (float)((BetaValueF * Lvalue * Lvalue) * (Lvalue / 3.06e4));

            MAP = MAPELTNS;

            for (mi = 0; mi < NUMENERG; mi++)
            {
                En[mi] = (float)(0.01 * (mi + 1));
            }
            TRARA1(Lvalue, BB0, En, ref flux, NUMENERG, descrelns);
            for (mi = 0; mi < NUMENERG; mi++)
            {
                mElect[mi] = Math.Pow(10, flux[mi]);
                //Console.Out.WriteLine(String.Format("{0}:{1}", En[mi], mElect[mi]));
            }

            MAP = MAPPRTNS;

            for (mi = 0; mi < NUMENERG; mi++)
            {
                En[mi] = (mi + 1) + 2;
            }
            TRARA1(Lvalue, BB0, En, ref flux, 10, descrprtns);
            for (mi = 0; mi < NUMENERG; mi++)
            {
                mProt[mi] = Math.Pow(10, flux[mi]);
                //Console.Out.WriteLine(String.Format("{0}:{1}", En[mi], mProt[mi]));
            }

            double[,] result = new double[4, NUMENERG + 10];
            for(int i = 0; i < NUMENERG; i++)
            {
                result[0,i] = 0.01 * ((double)i + 1);
                result[1,i] = mElect[i];
                result[2,i] = (i + 1) + 2;
                result[3,i] = mProt[i];
            }
            return result;
        }

    public void Dispose()
    {
        
    }
}

