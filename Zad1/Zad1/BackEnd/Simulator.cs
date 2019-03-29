﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Zad1.BackEnd {

    public class NEHtask
    {
        public int taskNumber;
        public int taskPriority;
    }



    static class Simulator {
        static int taskTimeSpan;
        static int taskID;
        static int taskStart;
        static int taskStop = 0;

        public static List<Task> firstMachineNEH = new List<Task>();
        public static List<Task> secondMachineNEH = new List<Task>();
        public static List<Task> thirdMachineNEH = new List<Task>();
        public static List<List<Task>> machineNEH = new List<List<Task>>();

        public static void simulateFullSearch(List<List<Task>> firstMachinePermuteResult, List<List<Task>> secondMachinePermuteResult) {
          
            //double numberOfPermutations = SpecialFunctions.Factorial(firstMachinePermuteResult[0].Count());
            int numberOfPermutations = firstMachinePermuteResult.Count();
            int numberOfTasks = firstMachinePermuteResult[0].Count();

            for (int j = 0; j < numberOfPermutations; j++)
            {
                for (int i = 0; i < numberOfTasks; i++)
                {

                    if (i == 0)
                    {
                        taskTimeSpan = firstMachinePermuteResult[j][i].TimeSpan;
                        taskID = firstMachinePermuteResult[j][i].ID;
                        taskStart = 0;
                        taskStop = taskTimeSpan + taskStart;
                        firstMachinePermuteResult[j][i].TaskStop = taskStop;
                        secondMachinePermuteResult[j][i].TaskStart = taskStop;
                        secondMachinePermuteResult[j][i].TaskStop = taskStop + secondMachinePermuteResult[j][i].TimeSpan;
                    }
                    else
                    {
                        taskStart = taskStop;
                        firstMachinePermuteResult[j][i].TaskStart = taskStart;
                        taskStop = taskStart + firstMachinePermuteResult[j][i].TimeSpan;
                        firstMachinePermuteResult[j][i].TaskStop = taskStop;

                        secondMachinePermuteResult[j][i].TaskStart = firstMachinePermuteResult[j][i].TaskStop;
                        secondMachinePermuteResult[j][i].TaskStop = secondMachinePermuteResult[j][i].TaskStart + secondMachinePermuteResult[j][i].TimeSpan;
                        
                         if ((firstMachinePermuteResult[j][i].TaskStop < secondMachinePermuteResult[j][i -1].TaskStop))
                         {
                            secondMachinePermuteResult[j][i].TaskStart = secondMachinePermuteResult[j][i - 1].TaskStop;
                            secondMachinePermuteResult[j][i].TaskStop = secondMachinePermuteResult[j][i].TaskStart + secondMachinePermuteResult[j][i].TimeSpan; 

                         }
                    }
                } 
            }
        }

        public static List<Task> simulateNEH(List<WorkCenter> workCenters)
        {
            //Stworzyc listę tasków totalWorkSpanPerTask, gdzie timeSpan zadania jest sumą wszystkich jego timeSpanów dla każdego workCentera np:
            List<Task> totalWorkSpanPerTask = new List<Task>(workCenters[0].Tasks);
            for (int i = 1; i < workCenters.Count; i++)
            {
                for (int j = 0; j < workCenters[i].Tasks.Count; j++)
                {
                    totalWorkSpanPerTask[j].TimeSpan += workCenters[i].Tasks[j].TimeSpan;
                }
            }

            //Posortować "descending"
            List<Task> SortedWorkSpanList = totalWorkSpanPerTask.OrderBy(o => o.TimeSpan).ToList();
            SortedWorkSpanList.Reverse();

            //Bazując na ID Tasków z tej listy dodawać do wirtualnych workcenterów pokolei po jednym tasku
            //następnie umieszczając dodany task na każdej z możliwych pozycji symulować działanie maszyn dla takiego zestawu tasków
            //Wybrać zestaw z najlepszym Cmax (TaskStop ostatniego zadania w ostatniej maszynie)
            //Powtórzyć aż ilość tasków w wirtualnych work centerach będzie równa ilości tasków

            List<WorkCenter> simulatedWorkCenters = new List<WorkCenter>(workCenters.Count);
            addAnotherTaskToSimulation(workCenters, SortedWorkSpanList, simulatedWorkCenters);
            #region addAnotherTaskToSimulation wytłumaczenie
            //if (simulatedWorkCenters[0].Tasks.Count != workCenters[0].Tasks.Count)
            //{
            //    int currentID = SortedWorkSpanList.First().ID;
            //    for (int i = 0; i < workCenters.Count; i++)
            //    {
            //        simulatedWorkCenters[i].Tasks.Add(new Task(workCenters[i].Tasks.SingleOrDefault(o => o.ID == currentID)));
            //    }
            //    SortedWorkSpanList.Remove(SortedWorkSpanList.Single(o => o.ID == currentID));
            //}
            #endregion
            //symulacja wyników, swapująć kolejno taska aż pokona drogę od końca, do początku, zapisanie sekwencji dla której wychodzi to najlepiej
            //Zapisanie do simulatedWorkCenters najlepszej sekwencji i powtórzenie działania(można wrzucić do while'a porównującego co samo, co ma if w obecnej metodzie addAnotherTaskToSimulation)


            //Opcjonalnie zrobić sytuacje gdy totalWorkSpan jest taki sam dla większej ilości zadań, ale to zostawimy na później

            //Zwrócić optymalną listę tasków
            return new List<Task>();
        }

        private static void addAnotherTaskToSimulation(List<WorkCenter> workCenters, List<Task> SortedWorkSpanList, List<WorkCenter> simulatedWorkCenters)
        {
            if (simulatedWorkCenters[0].Tasks.Count != workCenters[0].Tasks.Count)
            {
                int currentID = SortedWorkSpanList.First().ID;
                for (int i = 0; i < workCenters.Count; i++)
                {
                    simulatedWorkCenters[i].Tasks.Add(new Task(workCenters[i].Tasks.SingleOrDefault(o => o.ID == currentID)));
                }
                SortedWorkSpanList.Remove(SortedWorkSpanList.Single(o => o.ID == currentID));
            }
        }

        public static List<Task> simulateJohnson(List<WorkCenter> workCenters)
        {
            
            int min = 10000;
            int minTaskIndex = 0;
            int minMachineIndex = 0;
            List<Task> allTasks = new List<Task>();
            List<Task> taskSequenceM1 = new List<Task>();
            List<Task> taskSequenceM2 = new List<Task>();

            List<WorkCenter> localWorkCenter = workCenters.ConvertAll(workCenter => new WorkCenter(workCenter.Tasks));     

            bool tasksLeft = true;
            while(tasksLeft)
            {
                tasksLeft = false;
                foreach (WorkCenter machine in localWorkCenter)
                {
                    

                    foreach (Task task in machine.Tasks)
                    {  
                        if (task.TimeSpan <= min) ////////////////////////
                        {
                            min = task.TimeSpan;
                            minTaskIndex = machine.Tasks.IndexOf(task);
                            minMachineIndex = localWorkCenter.IndexOf(machine);
                        }
                        //TODO: what if they are equal
                    }
                }

                if (minMachineIndex == 0 && localWorkCenter[minMachineIndex].Tasks.Any())
                {
                    taskSequenceM1.Add(localWorkCenter[minMachineIndex].Tasks[minTaskIndex]);
                    localWorkCenter[0].Tasks.RemoveAt(minTaskIndex);
                    localWorkCenter[1].Tasks.RemoveAt(minTaskIndex);
                }
                else if (minMachineIndex == 1 && localWorkCenter[minMachineIndex].Tasks.Any())
                {
                    taskSequenceM2.Insert(0, localWorkCenter[minMachineIndex].Tasks[minTaskIndex]);
                    localWorkCenter[0].Tasks.RemoveAt(minTaskIndex);
                    localWorkCenter[1].Tasks.RemoveAt(minTaskIndex);
                }
                if (localWorkCenter[minMachineIndex].Tasks.Any())
                    {
                        tasksLeft = true;
                    }
                    min = 10000;
            }

            taskSequenceM1.AddRange(taskSequenceM2);
            
            return taskSequenceM1;
        }


        public static List<Task> simulateNEH(List<WorkCenter> workCenters)
        {
            List<NEHtask> priorityList = new List<NEHtask>();
            List<NEHtask> solutionList = new List<NEHtask>();
           
            //initialize priorityList
            foreach (Task task in workCenters[0].Tasks)
            {
                priorityList.Add(new NEHtask());
            }

            //calculate tasks priority
            foreach (WorkCenter machine in workCenters)
            {
                machineNEH.Add(new List<Task>());
                foreach(Task task in machine.Tasks)
                {
                    priorityList[machine.Tasks.IndexOf(task)].taskNumber = machine.Tasks.IndexOf(task);
                    priorityList[machine.Tasks.IndexOf(task)].taskPriority += task.TimeSpan;
                }
            }
            // sorting the priorities -largest last
            priorityList.Sort((p, q) => p.taskPriority.CompareTo(q.taskPriority));

            int currentBest = 10000000;
            int currentBestTime = 1000000;

            while (priorityList.Any())
            {
               
                for(int i = 0; i < solutionList.Count +1; i++)
                {
                    solutionList.Insert(i, priorityList.Last());
                    fillMachines(solutionList);
                    for(int j = 0; j < machineNEH.Count -1; j++)
                    {
                        sortPermutation(machineNEH[j], machineNEH[j +1], solutionList.Count(), machineNEH[j][0].TaskStart);
                        //sortPermutation(firstMachineNEH, secondMachineNEH, solutionList.Count(), 0);
                        //sortPermutation(secondMachineNEH, thirdMachineNEH, solutionList.Count(), secondMachineNEH[0].TaskStart);
                    }


                    if (machineNEH.Last()[solutionList.Count -1].TaskStop < currentBestTime)
                    {
                        currentBest = i;
                        currentBestTime = machineNEH.Last()[solutionList.Count - 1].TaskStop;
                    }
                   for(int k = 0; k < machineNEH.Count; k++)
                    {
                        machineNEH[k].Clear();
                    }
                    //firstMachineNEH.Clear();
                   // secondMachineNEH.Clear();
                   // thirdMachineNEH.Clear();
                    solutionList.RemoveAt(i);
                }
                solutionList.Insert(currentBest, priorityList.Last());
                priorityList.RemoveAt(priorityList.Count() - 1);
                currentBest = 10000000;
                currentBestTime = 1000000;
            }
            fillMachines(solutionList);
            for (int j = 0; j < machineNEH.Count - 1; j++)
            {
                sortPermutation(machineNEH[j], machineNEH[j + 1], solutionList.Count(), machineNEH[j][0].TaskStart);
                //sortPermutation(firstMachineNEH, secondMachineNEH, solutionList.Count(), 0);
                //sortPermutation(secondMachineNEH, thirdMachineNEH, solutionList.Count(), secondMachineNEH[0].TaskStart);
            }

            return new List<Task>();
        }


        public static void sortPermutation(List<Task> firstMachine, List<Task> secondMachine, int sizeOfTasks, int taskStart)
        {
            for (int i = 0; i < sizeOfTasks; i++)
            {

                if (i == 0)
                {
                    taskTimeSpan = firstMachine[i].TimeSpan;
                    taskID = firstMachine[i].ID;
                  //  taskStart = 0;
                    taskStop = firstMachine[i].TimeSpan + taskStart;
                    firstMachine[i].TaskStop = taskStop;

                    secondMachine[i].TaskStart = taskStop;
                    secondMachine[i].TaskStop = taskStop + secondMachine[i].TimeSpan;

                   // thirdMachine[i].TaskStart = secondMachine[i].TaskStop;
                   // thirdMachine[i].TaskStop = secondMachine[i].TaskStop + thirdMachine[i].TimeSpan;
                }
                else
                {
                    taskStart = taskStop;
                    firstMachine[i].TaskStart = taskStart;
                    taskStop = taskStart + firstMachine[i].TimeSpan;
                    firstMachine[i].TaskStop = taskStop;

                    secondMachine[i].TaskStart = firstMachine[i].TaskStop;
                    secondMachine[i].TaskStop = secondMachine[i].TaskStart + secondMachine[i].TimeSpan;

                   // thirdMachine[i].TaskStart = secondMachine[i].TaskStop;
                   // thirdMachine[i].TaskStop = thirdMachine[i].TaskStart + thirdMachine[i].TimeSpan;

                    if ((firstMachine[i].TaskStop <= secondMachine[i - 1].TaskStop))
                    {
                        secondMachine[i].TaskStart = secondMachine[i - 1].TaskStop;
                        secondMachine[i].TaskStop = secondMachine[i].TaskStart + secondMachine[i].TimeSpan;
                    }

                   // if ((secondMachine[i].TaskStop <= thirdMachine[i - 1].TaskStop))
                   // {
                       // thirdMachine[i].TaskStart = thirdMachine[i - 1].TaskStop;
                       // thirdMachine[i].TaskStop = thirdMachine[i].TaskStart + thirdMachine[i].TimeSpan;
                   // }
                }
            }
        }

        public static int indexOfTask(int IDvalue, List<Task> tasks)
        {
            foreach (Task task in tasks)
            {
                if (task.ID == IDvalue)
                {
                    return tasks.IndexOf(task);
                }
            }
            return -1;
        }

        public static void fillMachines(List<NEHtask> solutionList)
        {
            int timespan;
            foreach (NEHtask task in solutionList)
            {
                foreach(WorkCenter machine in Initializer.workCenters)
                {
                    int index = Initializer.workCenters.IndexOf(machine);
                    timespan = Initializer.workCenters[index].Tasks[indexOfTask(task.taskNumber, Initializer.workCenters[index].Tasks)].TimeSpan; 
                    machineNEH[index].Add(new Task(task.taskNumber, timespan));
                    //secondMachineNEH.Add(new Task(task.taskNumber, timespan2));
                   // thirdMachineNEH.Add(new Task(task.taskNumber, timespan3));
                }
            }
        }
        

    public static void checkResults(List<List<Task>> firstMachinePermuteResult, List<List<Task>> secondMachinePermuteResult) {
            Console.WriteLine("MACHINE ONE RESULTS :");
            foreach (List<Task> taskList in firstMachinePermuteResult)
            {
                Console.Write("[");
                foreach (Task task in taskList)
                {
                    Console.Write(task.TaskStart + " - " + task.TaskStop + ", ");
                }
                Console.WriteLine("]");
            }

            Console.WriteLine("\n MACHINE TWO RESULTS :");
            foreach (List<Task> taskList in secondMachinePermuteResult)
            {
                Console.Write("[");
                foreach (Task task in taskList)
                {
                    Console.Write(task.TaskStart + " - " + task.TaskStop + ", ");
                }
                Console.WriteLine("]");
            }

            Console.WriteLine("\n TASK PERMUTATIONS:");
            foreach (List<Task> taskList in firstMachinePermuteResult)
            {
                Console.Write("[");
                foreach (Task task in taskList)
                {
                    Console.Write(task.ID);
                }
                Console.WriteLine("]");
            }
            Console.ReadLine();
        }
    }
}
