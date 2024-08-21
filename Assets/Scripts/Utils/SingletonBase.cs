using System;

namespace Utils
{
   public class SingletonBase<T> where T : SingletonBase<T>
   {
      private static T _instance;
      private static readonly object _syncRoot = new object();

      public static T Instance
      {
         get
         {
            if (_instance == null)
            {
               lock (_syncRoot)
               {
                  if (_instance == null)
                     _instance = Activator.CreateInstance(typeof(T), true) as T;
               }
            }

            return _instance;
         }
      }
   
      protected SingletonBase()
      {
      
      }
   
   }
}
