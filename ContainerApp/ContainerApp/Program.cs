namespace Kontenery
{
    interface IHazardNotifier
    {
        void ZglosZagro(string containerSerialNumber);
    }

    class OverfillException : Exception
    {
        public OverfillException(string message) : base(message)
        {
            
        }
    }

    class Kontener : IHazardNotifier
    {
        private static int NastępnyNumer = 1;
        public string NrSeria { get; }
        public double WagaL { get; set;}
        public double Wysokosc { get; }
        public double Glebokosc { get; }
        public double WagaW { get; }
        public double Ladownosc { get; }
        public Kontener(double wagaL, double wysokosc, double wagaW, double glebokosc, double ladownosc)
        {
            WagaL = wagaL;
            Wysokosc = wysokosc;
            WagaW = wagaW;
            Glebokosc = glebokosc;
            NrSeria = GenerujNr();
            Ladownosc = ladownosc;
        }

        protected virtual string GenerujNr()
        {
            return $"KON-{NastępnyNumer++}";
        }
        public virtual void ZglosZagro(string containerSerialNumber)
        {
            Console.WriteLine($"Kontener {containerSerialNumber} jest zagrożony");
        }

        public virtual void Zaladuj(double wagal)
        {
            if ((wagal) > Ladownosc)
                throw new OverfillException("Przekroczono ładowność kontenera");
        }

        public virtual void Rozladuj()
        {
            WagaL = 0;
        }

        public virtual void Wypisz()
        {
            Console.WriteLine($"Masa ładunku: {WagaL}kg");
            Console.WriteLine($"Wysokosc: {Wysokosc}cm");
            Console.WriteLine($"Waga własna: {WagaW}kg");
            Console.WriteLine($"Głębokość: {Glebokosc}cm");
            Console.WriteLine($"Numer seryjny: {NrSeria}");
            Console.WriteLine($"Maksymalna ładowność: {Ladownosc}kg");
        }
    }

    class KontenerNaCiecz : Kontener
    {
        private static int NastepnaCiecz = 1;
        public enum TypCiecz {Niebezpieczny, Zwykly}
        public TypCiecz Typ { get; }

        public KontenerNaCiecz(double wagaL, double wysokosc, double wagaW, double glebokosc,
            double ladownosc, TypCiecz typ) : base(wagaL, wysokosc, wagaW, glebokosc, ladownosc)
        {
            Typ = typ;
        }

        protected override string GenerujNr()
        {
            return $"KON-L-{NastepnaCiecz++}";
        }

        public override void ZglosZagro(string containerSerialNumber)
        {
            Console.WriteLine($"Kontener na płyny {containerSerialNumber} jest zagrożony");
        }

        public override void Zaladuj(double wagaL)
        {
            base.Zaladuj(wagaL);
            if (Typ == TypCiecz.Niebezpieczny && (WagaL + wagaL) > Ladownosc * 0.5)
            {
                ZglosZagro(NrSeria);
                return;
            }
            else if (Typ == TypCiecz.Zwykly && (WagaL + wagaL) > Ladownosc * 0.9)
            {
                ZglosZagro(NrSeria);
                return;
            }

            WagaL += wagaL;
        }

        public override void Rozladuj()
        {
            base.Rozladuj();
            WagaL = 0;
        }

        public override void Wypisz()
        {
            base.Wypisz();
            Console.WriteLine($"Typ cieczy: {Typ}");
            Console.WriteLine();
        }
    }

    class KontenerNaGaz : Kontener
    {
        private static int NastepnyGaz= 1;
        public double Cisnienie { get; set; }
        public KontenerNaGaz(double wagaL, double wysokosc, double wagaW, double glebokosc,
            double ladownosc, double cisnienie) : base(wagaL, wysokosc, wagaW, glebokosc, ladownosc)
        {
            Cisnienie = cisnienie;
        }
        protected override string GenerujNr()
        {
            return $"KON-G-{NastepnyGaz++}";
        }
        public override void ZglosZagro(string containerSerialNumber)
        {
            Console.WriteLine($"Kontener na gaz {containerSerialNumber} jest zagrożony");
        }

        public override void Zaladuj(double wagal)
        {
            if (wagal > Ladownosc){
                ZglosZagro(NrSeria);
                return;
            }
            base.Zaladuj(wagal);
            WagaL = wagal;
        }
        public override void Rozladuj()
        {
            base.Rozladuj();
            WagaL = WagaL * 0.05;
        }
        public override void Wypisz()
        {
            base.Wypisz();
            Console.WriteLine($"Ciśnienie: {Cisnienie} atm");
            Console.WriteLine();
        }
    }

    class KontenerChlodniczy : Kontener
    {
        private static int NastepnyChlodzony = 1;
        public string TypProduktu { get; }
        public double TemperaturaKonstrukcyjna { get; }
        public double Temperatura { get; set; }
        public KontenerChlodniczy(double wagaL, double wysokosc, double wagaW, double glebokosc,
            double ladownosc, string typProduktu, double temperaturaKonstrukcyjna) : base(wagaL, wysokosc, wagaW, glebokosc, ladownosc)
        {
            TypProduktu = typProduktu;
            TemperaturaKonstrukcyjna = temperaturaKonstrukcyjna;
            Temperatura = TemperaturaKonstrukcyjna;
        }
        protected override string GenerujNr()
        {
            return $"KON-C-{NastepnyChlodzony++}";
        }
        public override void Zaladuj(double wagal)
        {
            base.Zaladuj(wagal);
            WagaL = wagal;
        }

        public override void Rozladuj()
        {
            base.Rozladuj();
            WagaL = 0;
        }

        public void ZmienTemp(double temperatura)
        {
            if (temperatura < TemperaturaKonstrukcyjna)
            {
                ZglosZagro(NrSeria);
                return;
            }

            Temperatura = temperatura;
        }
        public override void Wypisz()
        {
            base.Wypisz();
            Console.WriteLine($"Typ produktu: {TypProduktu}");
            Console.WriteLine($"Temperatura konstrukcyjna: {TemperaturaKonstrukcyjna}");
            Console.WriteLine($"Temperatura: {Temperatura}");
            Console.WriteLine();
        }
    }

    class Statek
    {
        public string Nazwa { get; }
        public List<Kontener> Zaladowane { get; } = new List<Kontener>();
        public double Vmax { get; }
        public int MaxKontenerow { get; }
        public double MaxLadownosc { get; }

        public Statek(string nazwa, double vmax, int maxKontenerow, double maxLadownosc)
        {
            Nazwa = nazwa;
            Vmax = vmax;
            MaxKontenerow = maxKontenerow;
            MaxLadownosc = maxLadownosc;
        }

        public void ZaladujKontener(Kontener kontener)
        {
            if (Zaladowane.Count >= MaxKontenerow)
                throw new Exception($"Statek {Nazwa} ma już za dużo kontenerów");
            double zaladowano = 0;
            foreach (var zaladowane in Zaladowane)
            {
                zaladowano += (zaladowane.WagaL + zaladowane.WagaW)/1000;
            }

            if (zaladowano > MaxLadownosc)
                throw new Exception($"Statek {Nazwa} jest już przeciążony");
            Zaladowane.Add(kontener);
        }
        public void ZaladujKontenery(List<Kontener> kontenery){
            foreach (var kontener in kontenery)
            {
                ZaladujKontener(kontener);
            }
        }

        public void RozladujKonener(string nrSeryjny)
        {
            var kontener = Zaladowane.Find(c => c.NrSeria == nrSeryjny);
            if (kontener != null)
                Zaladowane.Remove(kontener);
        }

        public void WymienKontener(string nrSeryjny1, Kontener kontenerZastep)
        {
            var kontenerZastepowany = Zaladowane.Find(c => c.NrSeria == nrSeryjny1);
            if (kontenerZastepowany != null)
            {
                int id = Zaladowane.IndexOf(kontenerZastepowany);
                Zaladowane[id] = kontenerZastep;
            }
            else
            {
                throw new Exception($"Brak kontenera o numerze seryjnym {nrSeryjny1}");
            }
        }

        public void PrzeniesKontener(Statek docelowy, string nrSeryjny)
        {
            var kontener = Zaladowane.Find(c => c.NrSeria == nrSeryjny);
            if (kontener != null)
            {
                Zaladowane.Remove(kontener);
                docelowy.ZaladujKontener(kontener);
            }
            else
            {
                throw new Exception($"Nie można przenieść kontenera o nr seryjnym {nrSeryjny}");
            }
        }

        public void WypiszInfo()
        {
            Console.WriteLine();
            Console.WriteLine($"Nazwa statku: {Nazwa}");
            Console.WriteLine($"Prędkość maksymalna: {Vmax} węzłów");
            Console.WriteLine($"Maksymalna ładowaność: {MaxLadownosc} ton");
            Console.WriteLine($"Maksymalna liczba kontenerów: {MaxKontenerow}");
            Console.WriteLine("Załadowane kontenery: ");
            foreach (var kontener in Zaladowane)
            {
                kontener.Wypisz();
                Console.WriteLine();
            }
        }
    }

    class Wykonaj
    {
        static void Main(string[] args)
        {
            KontenerNaCiecz kontenerNaCiecz = new KontenerNaCiecz(1000, 200, 500, 100,  5000, KontenerNaCiecz.TypCiecz.Niebezpieczny);
            KontenerNaGaz kontenerNaGaz = new KontenerNaGaz(1000, 200, 500, 100,  5000, 10);
            KontenerChlodniczy kontenerChlodniczy = new KontenerChlodniczy(1000, 200, 500, 100,  5000, "Kanapki", 8);
            kontenerNaCiecz.Zaladuj(1000);
            Statek tonacy = new Statek("Titanic", 18, 120, 650000);
            tonacy.ZaladujKontener(kontenerNaCiecz);
            List<Kontener> listaKontenerow = new List<Kontener>
            {
                new KontenerNaGaz(1500, 300, 700, 150, 7000, 8),
                new KontenerChlodniczy(2000, 250, 600, 120, 8000, "Mięso", 2)
            };
            tonacy.ZaladujKontenery(listaKontenerow);
            tonacy.RozladujKonener("KON-L-1");
            kontenerNaCiecz.Rozladuj();
            Statek nieTonocy = new Statek("Queen Elizabeth", 25, 20, 12000);
            tonacy.PrzeniesKontener(nieTonocy,"KON-G-2");
            kontenerNaGaz.Wypisz();
            tonacy.WypiszInfo();
        }
    }
}

