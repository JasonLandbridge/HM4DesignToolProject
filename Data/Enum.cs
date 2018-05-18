// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enum.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  This contains all the Enums for this project. </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HM4DesignTool.Data
{
    /// <summary>
    /// The level types enum.
    /// </summary>
    public enum LevelTypeEnum
    {
        /// <summary>
        /// The default option if the level type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The option when the level is a story level.
        /// </summary>
        Story = 1,

        /// <summary>
        /// The option for when the level is a normal bonus level.
        /// </summary>
        Bonus = 2,

        /// <summary>
        /// The option for when the level is a time trial bonus level.
        /// </summary>
        TimeTrial = 3,

        /// <summary>
        /// The option for when the level is a minigame bonus level.
        /// </summary>
        MiniGame = 4,

        /// <summary>
        /// The option for when the level is an Oliver level where 2 or less Olivers pop-up at the same time. 
        /// </summary>
        OliverOne = 5,

        /// <summary>
        /// The option for when the level is an Oliver level where nearly all Olivers pop-up at the same time. 
        /// </summary>
        OliverAll = 6
    }
}
