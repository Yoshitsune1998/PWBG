using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.SurvivorInventory;
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

        private static int CorrectAnswer(string word, Quiz quiz)
        {
            string[] words = word.Split();
            int correctWord = 0;
            string[] corrections = quiz.RightAnswer.Split();
            string temp = "";
            foreach (var w in words)
            {
                string a = w.ToLower();
                foreach (var c in corrections)
                {
                    string b = c.ToLower();
                    if (a.Equals(b))
                    {
                        temp += $"{a} ";
                        correctWord++;
                        if (usedWords.Contains(a)) continue;
                        usedWords.Add(a);
                    }
                }
            }
            if (usedWords.Contains(temp)) return 0;
            usedWords.Add($"{temp}\b");
            return correctWord;
        }

        private static bool CheckExistingAnswer(string word)
        {
            //misal jawab hataraku saibou // di simpan udah ada saibou platelet
            word = word.ToLower();
            var xword = word.Split();
            int xw = xword.Length;
            foreach (var used in usedWords)
            {
                int sametbh = 0;
                string temp = used.ToLower();
                var xtemp = temp.Split();
                int xt = xtemp.Length;
                foreach (var t in xtemp)
                {
                    foreach (var w in xword)
                    {
                        if (t.Equals(w)) sametbh++;
                    }
                }
                if (sametbh == xw) return true;
            }
            return false;
        }

        public static void ResetQuiz()
        {
            usedWords = new List<string>();
        }
        
        private static int CountingPoints(int correct, int despacito)
        {
            if (correct == despacito) return 7;
            return (7 * correct / despacito) + 1;
        }

        public static int CheckAnswer(string word, ulong idQuiz)
        {
            Quiz now = GetQuiz(idQuiz);
            if (now == null)
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(word)) return 0;
            if (CheckExistingAnswer(word))
            {
                return 0;
            }
            int numberCorrect = CorrectAnswer(word,now);
            if (numberCorrect==0)
            {
                return 0;
            }
            return CountingPoints(numberCorrect, now.WordContainInCorrectAnswer);
        }

        public static Quiz CreatingQuiz(string type, string imageUrl,string fullImage,string diff, string correct)
        {
            ulong stId = MainStorage.GetValueOf("LatestQuizId");
            
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in quizzes
                         where i.ID == Id
                         select i;
            var quiz = result.FirstOrDefault();
            if (quiz == null)
            {
                if (fullImage.Equals("x") || fullImage.Equals("X"))
                {
                    fullImage = "";
                }
                quiz = CreateQuiz(Id, type, imageUrl, fullImage, diff, correct);
            }
            return quiz;
        }

        private static Quiz CreateQuiz(ulong id, string type, string imageUrl, string fullImage,string diff, string correct)
        {
            var newQuiz = new Quiz()
            {
                ID = id,
                Type = type,
                Difficulty = diff,
                URL = imageUrl,
                RightAnswer = correct,
                FullImage = fullImage,
                Drop = new List<Item>(),
                Hints = new List<string>()
             };
            quizzes.Add(newQuiz);
            MainStorage.ChangeData("LatestQuizId", id);
            SaveQuizzes();
            return newQuiz;
        }

        public static void AddingHints(Quiz quiz, string hints)
        {
            quiz.Hints.Add(hints);
            SaveQuizzes();
        }

        public static void AddingDrops(Quiz quiz, Item item)
        {
            quiz.Drop.Add(item);
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
