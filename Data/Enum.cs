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

    /// <summary>
    /// The treatment type enum.
    /// </summary>
    public enum TreatmentTypeEnum
    {
        /// <summary>
        /// The default option if the level type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The quick treatment, a single click treatment which will be quickly treated
        /// </summary>
        Quick = 1,

        /// <summary>
        /// The gesture treatment, this requires the player to perform a gesture, such as drawing a circle or swiping to the right along a bar. This does not require a product to be carried by Allison.  
        /// </summary>
        Gesture = 2,

        /// <summary>
        /// The ingredient which needs to be picked and re-stocked.
        /// </summary>
        Ingredient = 3,

        /// <summary>
        /// The single product, this requires Allison to pick-up a product from a table.
        /// </summary>
        SingleProduct = 4,

        /// <summary>
        /// The combo product, combine two products to create a Combo Product to resolve this treatment.
        /// </summary>
        ComboProduct = 5,

        /// <summary>
        /// The cook product, this requires Allison to pick-up a product from a table and cook it.
        /// </summary>
        CookProduct = 6,

        /// <summary>
        /// The minigame, this requires the player to play a minigame to proceed to the next treatment, such as stitching a wound or performing a brain scan and finding the troubled areas. 
        /// </summary>
        Minigame = 7,
    }

}
