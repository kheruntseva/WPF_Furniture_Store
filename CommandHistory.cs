//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.Linq;
//namespace mywpf
//{
//    public class CommandHistory
//    {
//        private readonly Stack<ObservableCollection<RectangleItem>> _undoStack = new Stack<ObservableCollection<RectangleItem>>();
//        private readonly Stack<ObservableCollection<RectangleItem>> _redoStack = new Stack<ObservableCollection<RectangleItem>>();

//        public void Push(ObservableCollection<RectangleItem> currentState)
//        {
//            if (currentState == null)
//            {
//                Debug.WriteLine("CommandHistory.Push: currentState is null");
//                return;
//            }

//            var clone = new ObservableCollection<RectangleItem>(
//                currentState.Select(item => new RectangleItem
//                {
//                    Name = item.Name,
//                    Description = item.Description,
//                    Price = item.Price,
//                    Category = item.Category,
//                    Color = item.Color,
//                    Image = item.Image,
//                    Availability = item.Availability
//                }));

//            _undoStack.Push(clone);
//            _redoStack.Clear();
//            Debug.WriteLine($"CommandHistory.Push: Added state with {clone.Count} items to undo stack");
//        }

//        public ObservableCollection<RectangleItem> Undo(ObservableCollection<RectangleItem> currentState)
//        {
//            if (!CanUndo)
//            {
//                Debug.WriteLine("CommandHistory.Undo: Cannot undo, stack is empty");
//                return currentState;
//            }

//            if (currentState != null)
//            {
//                var clone = new ObservableCollection<RectangleItem>(
//                    currentState.Select(item => new RectangleItem
//                    {
//                        Name = item.Name,
//                        Description = item.Description,
//                        Price = item.Price,
//                        Category = item.Category,
//                        Color = item.Color,
//                        Image = item.Image,
//                        Availability = item.Availability
//                    }));
//                _redoStack.Push(clone);
//                Debug.WriteLine($"CommandHistory.Undo: Pushed current state with {clone.Count} items to redo stack");
//            }

//            var result = _undoStack.Pop();
//            Debug.WriteLine($"CommandHistory.Undo: Popped state with {result.Count} items from undo stack");
//            return result;
//        }

//        public ObservableCollection<RectangleItem> Redo(ObservableCollection<RectangleItem> currentState)
//        {
//            if (!CanRedo)
//            {
//                Debug.WriteLine("CommandHistory.Redo: Cannot redo, stack is empty");
//                return currentState;
//            }

//            if (currentState != null)
//            {
//                var clone = new ObservableCollection<RectangleItem>(
//                    currentState.Select(item => new RectangleItem
//                    {
//                        Name = item.Name,
//                        Description = item.Description,
//                        Price = item.Price,
//                        Category = item.Category,
//                        Color = item.Color,
//                        Image = item.Image,
//                        Availability = item.Availability
//                    }));
//                _undoStack.Push(clone);
//                Debug.WriteLine($"CommandHistory.Redo: Pushed current state with {clone.Count} items to undo stack");
//            }

//            var result = _redoStack.Pop();
//            Debug.WriteLine($"CommandHistory.Redo: Popped state with {result.Count} items from redo stack");
//            return result;
//        }

//        public bool CanUndo => _undoStack.Count > 0;
//        public bool CanRedo => _redoStack.Count > 0;

//        public void Clear()
//        {
//            _undoStack.Clear();
//            _redoStack.Clear();
//            Debug.WriteLine("CommandHistory.Clear: Both stacks cleared");
//        }
//    }
//}