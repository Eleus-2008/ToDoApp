using System.Collections;
using System.Collections.Generic;

namespace ToDoApp.ViewModel
{
    public class TasksSorter : IComparer
    {
        public int Compare(object xObj, object yObj)
        {
            var x = xObj as TaskViewModel;
            var y = yObj as TaskViewModel;
            if (!x.IsActual && y.IsActual)
            {
                return 1;
            }

            if (x.IsActual && !y.IsActual)
            {
                return -1;
            }
            
            if (!x.IsExpired && y.IsExpired)
            {
                return 1;
            }

            if (x.IsExpired && !y.IsExpired)
            {
                return -1;
            }
            
            if (!x.Date.HasValue && y.Date.HasValue)
            {
                return 1;
            }

            if (x.Date.HasValue && !y.Date.HasValue)
            {
                return -1;
            }
            
            if (x.Date.HasValue && y.Date.HasValue)
            {
                if (x.Date > y.Date)
                {
                    return 1;
                }
                
                if (x.Date < y.Date)
                {
                    return -1;
                }

                if (x.Date == y.Date)
                {
                    if (!x.TimeOfBeginning.HasValue && y.TimeOfBeginning.HasValue)
                    {
                        return 1;
                    }

                    if (x.TimeOfBeginning.HasValue && !y.TimeOfBeginning.HasValue)
                    {
                        return -1;
                    }
                    
                    if (x.TimeOfBeginning.HasValue && y.TimeOfBeginning.HasValue)
                    {
                        if (x.TimeOfBeginning > y.TimeOfBeginning)
                        {
                            return 1;
                        }
                
                        if (x.TimeOfBeginning < y.TimeOfBeginning)
                        {
                            return -1;
                        }
                    }
                }
            }

            if (x.Priority < y.Priority)
            {
                return 1;
            }
            
            if (x.Priority > y.Priority)
            {
                return -1;
            }
            
            return 0;
        }
    }
}