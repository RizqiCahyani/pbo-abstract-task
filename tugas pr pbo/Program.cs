using System;

namespace PertarunganRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.SelectRobot();
            game.StartGame();
        }
    }

    public class Game
    {
        Robot Player;
        string JenisRobot;
        Robot Enemy;
        Kemampuan abilityPlayer;
        Kemampuan abilityEnemy;

        int turn = 1;

        public Game()
        {
            this.Enemy = new BossRobot();
            this.abilityEnemy = new Kemampuan();
            this.abilityPlayer = new Kemampuan();
        }

        public void SelectRobot()
        {
            string opsi;
            do
            {
                Console.WriteLine("Silahkan Memilih Robot yang Akan dimainkan\n1. Robot Electric\n2. Robot Plasma");
                Console.Write("Pilihan Robot : ");
                opsi = Console.ReadLine();
                Console.Clear();
            } while (opsi != "1" && opsi != "2");

            switch (opsi)
            {
                case "1":
                    this.Player = new RobotElectric();
                    this.JenisRobot = "Robot Electric";
                    break;
                case "2":
                    this.Player = new RobotPlasma();
                    this.JenisRobot = "Robot Plasma";
                    break;
            }
        }

        public void StartGame()
        {
            while (true)
            {
                this.ReduceCooldown();
                this.PlayerSession();

                if (Player.energi <= 0 || Enemy.energi <= 0 || turn == 12)
                {
                    Console.WriteLine("Game Berakhir");
                    Console.WriteLine($"Game Dimenangkan Oleh {this.GetWinner()}");
                    break;
                }

                Console.Write("Tekan Enter Untuk Lanjut");
                Console.ReadLine();
                this.EnemySession();

                if (Player.energi <= 0 || Enemy.energi <= 0 || turn == 12)
                {
                    Console.WriteLine("Game Berakhir");
                    Console.WriteLine($"Game Dimenangkan Oleh {this.GetWinner()}");
                    break;
                }

                this.turn++;
                Console.Write("Tekan Enter Untuk Lanjut");
                Console.ReadLine();
            }
        }

        public void PlayerSession()
        {
            bool aksiBerhasil = false;
            string ability = this.JenisRobot == "Robot Plasma" ? "Plasma" : "Electric";
            string opsi;

            do
            {
                Console.Clear();
                Console.WriteLine($"Turn : {this.turn}");
                Player.CekInformasi();
                Console.WriteLine("1. Serang\n2. Gunakan Kemampuan Spesial\n3. Aktifkan Pertahanan Tambahan");
                Console.Write("Pilihan : ");
                opsi = Console.ReadLine();
            } while (opsi != "1" && opsi != "2" && opsi != "3");

            switch (opsi)
            {
                case "1":
                    Player.Serang(Enemy);
                    aksiBerhasil = true;
                    break;
                case "2":
                    aksiBerhasil = Player.GunakanKemampuan(abilityPlayer, Enemy, ability);
                    break;
                case "3":
                    aksiBerhasil = Player.GunakanKemampuan(abilityPlayer, Player, "Shield");
                    break;
            }

            if (!aksiBerhasil)
            {
                this.PlayerSession();
            }
        }

        public void EnemySession()
        {
            Console.Clear();
            Enemy.CekInformasi();
            Random random = new Random();
            bool aksiBerhasil = false;
            string[] aksi = { "Electric", "Plasma", "Serang" };
            string opsi = aksi[random.Next(aksi.Length)];

            switch (opsi)
            {
                case "Serang":
                    Enemy.Serang(Player);
                    aksiBerhasil = true;
                    break;
                case "Electric":
                    aksiBerhasil = Enemy.GunakanKemampuan(abilityEnemy, Player, "Electric");
                    break;
                case "Plasma":
                    aksiBerhasil = Enemy.GunakanKemampuan(abilityEnemy, Player, "Plasma");
                    break;
            }

            if (!aksiBerhasil)
            {
                this.EnemySession();
            }
        }

        public string GetWinner()
        {
            return Player.energi > Enemy.energi ? Player.nama : Enemy.nama;
        }

        public void ReduceCooldown()
        {
            // Mengurangi cooldown skill baik player maupun enemy
            abilityPlayer.ReduceCooldown();
            abilityEnemy.ReduceCooldown();
        }
    }

    public abstract class Robot
    {
        public string nama;
        public int energi, armor, serangan;

        public abstract bool GunakanKemampuan(Kemampuan kemampuan, Robot target, string skill);

        public void CekInformasi()
        {
            Console.WriteLine($"Nama: {nama}\nEnergi: {energi}\nArmor: {armor}\nSerangan: {serangan}");
        }

        public void Serang(Robot target)
        {
            Console.WriteLine($"{nama} menyerang {target.nama}!");
            if (target.armor > serangan)
            {
                target.armor -= serangan;
            }
            else
            {
                target.energi -= serangan - target.armor;
                target.armor = 0;
            }
        }
    }

    public class BossRobot : Robot
    {
        public BossRobot()
        {
            this.nama = "Boss Robot";
            this.energi = 200;
            this.armor = 50;
            this.serangan = 40;
        }

        public override bool GunakanKemampuan(Kemampuan kemampuan, Robot target, string skill)
        {
            return kemampuan.GunakanSkill(this, target, skill);
        }
    }

    public class RobotElectric : Robot
    {
        public RobotElectric()
        {
            this.nama = "Robot Electric";
            this.energi = 100;
            this.armor = 30;
            this.serangan = 25;
        }

        public override bool GunakanKemampuan(Kemampuan kemampuan, Robot target, string skill)
        {
            return kemampuan.GunakanSkill(this, target, skill);
        }
    }

    public class RobotPlasma : Robot
    {
        public RobotPlasma()
        {
            this.nama = "Robot Plasma";
            this.energi = 100;
            this.armor = 35;
            this.serangan = 30;
        }

        public override bool GunakanKemampuan(Kemampuan kemampuan, Robot target, string skill)
        {
            return kemampuan.GunakanSkill(this, target, skill);
        }
    }

    public class Kemampuan
    {
        private int cooldownElectric = 2;
        private int cooldownPlasma = 3;
        private int cooldownShield = 4;

        public bool GunakanSkill(Robot pengguna, Robot target, string skill)
        {
            switch (skill)
            {
                case "Electric":
                    if (cooldownElectric > 0) return false;
                    Console.WriteLine($"{pengguna.nama} menggunakan Serangan Electric!");
                    target.energi -= 30;
                    cooldownElectric = 2;
                    return true;
                case "Plasma":
                    if (cooldownPlasma > 0) return false;
                    Console.WriteLine($"{pengguna.nama} menggunakan Serangan Plasma!");
                    target.energi -= 40;
                    cooldownPlasma = 3;
                    return true;
                case "Shield":
                    if (cooldownShield > 0) return false;
                    Console.WriteLine($"{pengguna.nama} menggunakan Pertahanan Super!");
                    pengguna.armor += 20;
                    cooldownShield = 4;
                    return true;
            }
            return false;
        }

        public void ReduceCooldown()
        {
            if (cooldownElectric > 0) cooldownElectric--;
            if (cooldownPlasma > 0) cooldownPlasma--;
            if (cooldownShield > 0) cooldownShield--;
        }
    }
}
