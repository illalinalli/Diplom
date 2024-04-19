using Amazon.Runtime.Internal.Transform;
using System;

namespace HomelessAnimalsDiplom.Models
{
    public class TfidfVectorizer
    {
        
        public List<List<double>> FitTransform(List<string> itemsFeatures)
        {
            List<List<double>> tfidfMatrix = new List<List<double>>();

            foreach (var itemFeatures in itemsFeatures)
            {
                List<double> tfidfVector = new List<double>();
                string[] words = itemFeatures.Split(' ');

                // Compute TF (Term Frequency)
                Dictionary<string, int> wordCount = new Dictionary<string, int>();
                foreach (var word in words)
                {
                    if (wordCount.ContainsKey(word))
                        wordCount[word]++;
                    else
                        wordCount[word] = 1;
                }

                // Compute IDF (Inverse Document Frequency)
                //Dictionary<string, double> wordIdf = new Dictionary<string, double>();
                foreach (var word in wordCount.Keys)
                {
                    int docsWithWord = itemsFeatures.Count(doc => doc.Contains(word));
                    double idf = Math.Log((double)itemsFeatures.Count / (docsWithWord)); // 1 +
                    wordIdf[word] = idf;
                }

                // Compute TF-IDF
                foreach (var word in words)
                {
                    double tfidf = wordCount[word] * wordIdf[word];
                    tfidfVector.Add(tfidf);
                    //wordIdf.Add(word, tfidf);
                }

                tfidfMatrix.Add(tfidfVector);
            }
            return tfidfMatrix;
        }

        public List<List<double>> Transform(List<string> usersFeatures)
        {
            List<List<double>> tfidfMatrix = new List<List<double>>();

            foreach (var userFeatures in usersFeatures)
            {
                List<double> tfidfVector = new List<double>();
                string[] words = userFeatures.Split(' ');

                // Use the IDF values computed during FitTransform for TF-IDF calculation
                // You can store the IDF values in the TfidfVectorizer class for reuse

                // Compute TF (Term Frequency) for user features
                Dictionary<string, int> wordCount = new Dictionary<string, int>();
                foreach (var word in words)
                {
                    if (wordCount.ContainsKey(word))
                        wordCount[word]++;
                    else
                        wordCount[word] = 1;
                }

                // Compute TF-IDF using IDF values from FitTransform
                foreach (var word in words)
                {
                    if (wordIdf.ContainsKey(word))
                    {
                        double tfidf = wordCount[word] * wordIdf[word];
                        tfidfVector.Add(tfidf);
                    }
                    else
                    {
                        tfidfVector.Add(0); // Word not present in training data, so IDF is 0
                    }
                }

                tfidfMatrix.Add(tfidfVector);
            }

            return tfidfMatrix;
        }

        private Dictionary<string, double> wordIdf = new Dictionary<string, double>(); // Store IDF values for reuse
        
    }
}

