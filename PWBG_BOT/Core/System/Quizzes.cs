using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.PlayerInventory;

namespace PWBG_BOT.Core.System
{
    public static class Quizzes
    {
        private static List<Quiz> quizzes;

        private static string QuizzesFile = "Resources/quizzes.json";

        static Quizzes()
        {
            if (DataStorage.SaveExist(QuizzesFile))
            {
                quizzes = DataStorage.LoadQuiz(QuizzesFile).ToList();
            }
            else
            {
                quizzes = new List<Quiz>();
                SaveQuizzes();
            }
        }

        public static Quiz GetQuiz(ulong id)
        {
            var result = from q in quizzes
                         where q.ID == id
                         select q;
            var quiz = result.FirstOrDefault();
            return quiz;
        }

        public static List<Quiz> GetQuizzes()
        {
            return quizzes;
        }

        public static void SaveQuizzes()
        {
            DataStorage.SaveQuizzes(quizzes, QuizzesFile);
        }

        private static Quiz CreatingQuiz(string type, string imageUrl,string diff, string dropName, string correct, string[] hints)
        {
            ulong stId = MainStorage.GetValueOf("LatestQuizId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in quizzes
                         where i.ID == Id
                         select i;
            var quiz = result.FirstOrDefault();
            if (quiz == null) quiz = CreateQuiz(Id, type, imageUrl, diff, dropName, correct, hints);
            return quiz;
        }

        private static Quiz CreateQuiz(ulong id, string type, string imageUrl, string diff, string dropName, string correct, string[] hints)
        {
            var newQuiz = new Quiz()
            {
                ID = id,
                Type = type,
                Difficulty = diff,
                Drop = Drops.PackageOfItem(dropName),
                ImageURL = imageUrl,
                RightAnswer = correct,
                Hints = hints.OfType<string>().ToList()
             };
            quizzes.Add(newQuiz);
            MainStorage.ChangeData("LatestQuizId", id);
            SaveQuizzes();
            return newQuiz;
        }

    }
}
