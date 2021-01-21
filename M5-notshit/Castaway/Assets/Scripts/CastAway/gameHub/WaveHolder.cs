namespace gameHub
{
    [System.Serializable]
    public class WaveHolder
    {
        public int amountOfEnemies;
        public int amountOfSpecialEnemies;
        public int minComboAmount;
        public int maxComboAmount;
        public int minWaveSize;
        public int maxWaveSize;
        public float minTimeBetweenSpawns;
        public float maxTimeBetweenSpawns;
        public float zombieSpeed;
        public float intervalComboUp;
        public float intervalWaveUp;
    }
}