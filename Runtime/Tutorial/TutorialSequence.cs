using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// A sequence of tutorial steps.
    /// </summary>
    [Serializable]
    public class TutorialSequence
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private int _priority;
        [SerializeField] private bool _canSkipAll;
        [SerializeField] private List<TutorialStep> _steps = new();

        /// <summary>
        /// Unique identifier for this sequence.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Display name.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Priority (higher = shown first).
        /// </summary>
        public int Priority => _priority;

        /// <summary>
        /// Whether the entire sequence can be skipped.
        /// </summary>
        public bool CanSkipAll => _canSkipAll;

        /// <summary>
        /// Steps in this sequence.
        /// </summary>
        public IReadOnlyList<TutorialStep> Steps => _steps;

        /// <summary>
        /// Number of steps.
        /// </summary>
        public int StepCount => _steps.Count;

        public TutorialSequence(string id, string name = null, int priority = 0, bool canSkipAll = true)
        {
            _id = id;
            _name = name;
            _priority = priority;
            _canSkipAll = canSkipAll;
        }

        /// <summary>
        /// Adds a step to the sequence.
        /// </summary>
        public TutorialSequence AddStep(TutorialStep step)
        {
            _steps.Add(step);
            return this;
        }

        /// <summary>
        /// Gets a step by index.
        /// </summary>
        public TutorialStep GetStep(int index)
        {
            if (index < 0 || index >= _steps.Count) return null;
            return _steps[index];
        }

        /// <summary>
        /// Gets a step by ID.
        /// </summary>
        public TutorialStep GetStep(string stepId)
        {
            return _steps.Find(s => s.Id == stepId);
        }
    }
}
