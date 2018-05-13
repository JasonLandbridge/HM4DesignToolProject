// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HM4DesignTool.Utilities
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// The command handler used to relay Commands from the Front-End to the Back-End.
    /// </summary>
    public class CommandHandler : System.Windows.Input.ICommand
    {
        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action action;

        /// <summary>
        /// The can execute.
        /// </summary>
        private readonly bool canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="action">
        /// The Command Action.
        /// </param>
        /// <param name="canExecute">
        /// Check if this Command is allowed to be executed
        /// </param>
        public CommandHandler(Action action, bool canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        /// <inheritdoc />
        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Boolean" />.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute;
        }

        /// <inheritdoc />
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.action();
        }
    }
}
