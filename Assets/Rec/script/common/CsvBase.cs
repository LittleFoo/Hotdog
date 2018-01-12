//
// /********************************************************
// * 
// *　　　　　　Copyright (c) 2015  Feiyu
// *  
// * Author		: Binglei Gong</br>
// * Date		: 15-12-8下9:38</br>
// * Declare	: </br>
// * Version	: 1.0.0</br>
// * Summary	: create</br>
// *
// *
// *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common
{
    #region csvfilename
    public class CsvConfig
    {
        public static string CSV_LEVEL = "Data/Level";
		public static string CSV_BONUS = "Data/Bonus";
		public static List<T> stringToIntArray<T>(string s)
        {
            if(s == "")
                return null;

			string[] ss = s.Split(";"[0]);
            List<T> result = new List<T>(ss.Length);

            for(int i = 0; i < ss.Length; i++)
				result.Add((T)Convert.ChangeType( ss[i], typeof(T)));

            return result;
        }
    }
    #endregion
    #region CSVBASE
    public interface ICsvBase
    {
        Type getUnitType();
        void addObj(String idx, object obj);
    }
    #endregion

    #region CsvLevel
    public class CsvLevel : ICsvBase
    {
        private Dictionary<int, CsvLevelUnit> units;
        public CsvLevel()
        {
			units = new Dictionary<int, CsvLevelUnit>();
        }
        public Type getUnitType()
        {
            return typeof(CsvLevelUnit);
        }
        public void addObj(String idx, object obj)
        {
			CsvLevelUnit unit = (CsvLevelUnit)obj;
			units.Add(Convert.ToInt32(idx), unit);
        }
        public CsvLevelUnit getObj(int idx)
        {
            CsvLevelUnit unit;
            units.TryGetValue(idx, out unit);
            if (unit == null)
                throw new Exception("CsvLevel 不存在 idx = " + idx.ToString());
            return unit;
        }
    }

    public class CsvLevelUnit
    {
		private int id;
		public int ID { get { return id; } set { id = value; } }
		private string monsters;
		public string Monsters { get { return monsters; } set { monsters = value; } }
		private string bonus;
		public string Bonus { get { return bonus; } set { bonus = value; } }
		private float speedRate;
		public float SpeedRate { get { return speedRate; } set { speedRate = value; } }
		private int bonusNum;
		public int BonusNum { get { return bonusNum; } set { bonusNum = value; } }
		public  List<int> bonusIds;
		public List<Coord> monsterDatas;
    }
    #endregion

	#region CsvLevel
	public class CsvBonus : ICsvBase
	{
		private Dictionary<int, List<CsvBonusUnit>> units;
		public CsvBonus()
		{
			units = new Dictionary<int, List<CsvBonusUnit>>();
		}
		public Type getUnitType()
		{
			return typeof(CsvBonusUnit);
		}
		public void addObj(String idx, object obj)
		{
			int idxNum = Convert.ToInt32(idx);
			CsvBonusUnit unit = (CsvBonusUnit)obj;
			List<CsvBonusUnit> unitList;
			units.TryGetValue(idxNum, out unitList);
			if(unitList == null)
			{
				unitList = new List<CsvBonusUnit>();
				units.Add(idxNum, unitList);
			}
			unitList.Add(unit);
		}
		public CsvBonusUnit getObj(int idx, int idx2 = 0)
		{
			List< CsvBonusUnit> unitList;
			units.TryGetValue(idx, out unitList);
			if (unitList == null || idx2>unitList.Count)
				throw new Exception("CsvBonus 不存在 idx = " + idx.ToString());
			return unitList[idx2];
		}
	}

	public class CsvBonusUnit
	{
		private int id;
		public int ID { get { return id; } set { id = value; } }

		private string bonusName;
		public string BonusName { get { return bonusName; } set { bonusName = value; } }

		private int level;
		public int Level { get { return level; } set { level = value; } }

		private float effect;
		public float Effect { get { return effect; } set { effect = value; } }

		private float time;
		public float Time { get { return time; } set { time = value; } }
	}
	#endregion
}
