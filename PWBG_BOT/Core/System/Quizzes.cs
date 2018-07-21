using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.PlayerInventory;
using Discord.WebSocket;

namespace PWBG_BOT.Core.System
{
    public static class Quizzes
    {
        private static List<Quiz> quizzes;
        private static List<string> usedWords;

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

        private static uint CorrectAnswer(string word, Quiz quiz)
        {
            usedWords.Add(word);
            string[] words = word.Split();
            uint correctWord = 0;
            string[] corrections = quiz.RightAnswer.Split();
            foreach (var w in words)
            {
                foreach (var c in corrections)
                {
                    if (w.ToLower().Equals(c.ToLower()))
                    {
                        usedWords.Add(w.ToLower());
                        correctWord++;
                    }
                }
            }
            return correctWord;
        }

        private static bool CheckExistedAnswer(string word)
        {
            word = word.ToLower();
            Console.WriteLine(word);
            if (usedWords.Contains(word))
            {
                return true;
            }
            return false;
        }

        public static void ResetQuiz()
        {
            usedWords = new List<string>();
        }
        
        private static uint CountingPoints(uint correct, uint despacito)
        {
            if (correct == despacito) return 10;
            return ((uint)10 * correct / despacito) + 1;
        }

        public static uint CheckAnswer(string word, ulong idQuiz)
        {
            Quiz now = GetQuiz(idQuiz);
            if (now == null)
            {
                return 0;
            }
            if (CheckExistedAnswer(word))
            {
                return 0;
            }
            uint numberCorrect = CorrectAnswer(word,now);
            if (numberCorrect==0)
            {
                return 0;
            }
            return CountingPoints(numberCorrect, now.WordContainInCorrectAnswer);
        }

        public static Quiz CreatingQuiz(string type, string imageUrl,string diff, ulong dropId, string correct)
        {
            ulong stId = MainStorage.GetValueOf("LatestQuizId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in quizzes
                         where i.ID == Id
                         select i;
            var quiz = result.FirstOrDefault();
            if (quiz == null) quiz = CreateQuiz(Id, type, imageUrl, diff, dropId, correct);
            return quiz;
        }

        private static Quiz CreateQuiz(ulong id, string type, string imageUrl, string diff, ulong dropId, string correct)
        {
            var newQuiz = new Quiz()
            {
                ID = id,
                Type = type,
                Difficulty = diff,
                Drop = Drops.PackageOfItem(dropId),
                URL = imageUrl,
                RightAnswer = correct,
             };
            quizzes.Add(newQuiz);
            MainStorage.ChangeData("LatestQuizId", id);
            SaveQuizzes();
            return newQuiz;
        }

        public static void AddingHints(Quiz quiz, List<string> hints)
        {
            quiz.Hints = hints;
            SaveQuizzes();
        }

        public static async Task GiveDrops(List<SocketUser> users, SocketTextChannel channel)
        {
            Quiz now = GlobalVar.Selected;

            if (now == null) return;
            
            foreach (var u in users)
            {
                foreach (var i in now.Drop)
                {
                    await Inventories.GiveItem(u,i,channel);
                }
            }
        }

    }
}
