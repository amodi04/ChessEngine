﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using static Engine.AI.AISettings;

namespace Chess.GUI.Views
{
    /// <summary>
    /// This class handles UI logic for the AISettings window
    /// </summary>
    public class AISettingsWindow : Window
    {
        // Member fields
        private readonly AISettingElement _depth;
        private readonly AISettingElement _checkmateBonus;
        private readonly AISettingElement _checkBonus;
        private readonly AISettingElement _castleBonus;
        private readonly AISettingElement _mobilityMultiplier;
        private readonly AISettingElement _pieceMultiplier;
        private readonly AISettingElement _attackMultiplier;
        private readonly AISettingElement _depthMultiplier;
        private readonly AISettingElement _twoBishopsBonus;
        private readonly AISettingElement _twoRooksBonus;

        private readonly AISettingElement _pawnValue;
        private readonly AISettingElement _knightValue;
        private readonly AISettingElement _bishopValue;
        private readonly AISettingElement _rookValue;
        private readonly AISettingElement _queenValue;

        private readonly CheckBox _useBetterEvaluator;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public AISettingsWindow()
        {
            // Initialise components first
            InitializeComponent();
            
            // Find all of the elements required from the xaml
            _depth = this.Find<AISettingElement>("Depth");
            _checkmateBonus = this.Find<AISettingElement>("CheckmateBonus");
            _checkBonus = this.Find<AISettingElement>("CheckBonus");
            _castleBonus = this.Find<AISettingElement>("CastleBonus");
            _mobilityMultiplier = this.Find<AISettingElement>("MobilityMultiplier");
            _pieceMultiplier = this.Find<AISettingElement>("PieceMultiplier");
            _attackMultiplier = this.Find<AISettingElement>("AttackMultiplier");
            _depthMultiplier = this.Find<AISettingElement>("DepthMultiplier");
            _twoBishopsBonus = this.Find<AISettingElement>("TwoBishopsBonus");
            _twoRooksBonus = this.Find<AISettingElement>("TwoRooksBonus");
            
            _pawnValue = this.Find<AISettingElement>("PawnValue");
            _knightValue = this.Find<AISettingElement>("KnightValue");
            _bishopValue = this.Find<AISettingElement>("BishopValue");
            _rookValue = this.Find<AISettingElement>("RookValue");
            _queenValue = this.Find<AISettingElement>("QueenValue");

            _useBetterEvaluator = this.Find<CheckBox>("UseBetterEvaluator");
            
            // Set the values, passing in the current AISettings data
            SetValues(Depth, CheckmateBonus, CheckBonus, CastleBonus, MobilityMultiplier, PieceMultiplier,
                AttackMultiplier, DepthMultiplier, TwoBishopsBonus, TwoRooksBonus, PawnValue, KnightValue, BishopValue,
                RookValue, QueenValue, UseBetterEvaluator);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Sets the values on the slider from the passed in data.
        /// </summary>
        /// <param name="depth">Depth value.</param>
        /// <param name="checkmateBonus">Checkmate Bonus value.</param>
        /// <param name="checkBonus">Check bonus value.</param>
        /// <param name="castleBonus">Castle bonus value.</param>
        /// <param name="mobilityMultiplier">Mobility multiplier value.</param>
        /// <param name="pieceMultiplier">Piece multiplier value.</param>
        /// <param name="attackMultiplier">Attack multiplier value.</param>
        /// <param name="depthMultiplier">Depth multiplier value.</param>
        /// <param name="twoBishopsBonus">Two bishops bonus value.</param>
        /// <param name="twoRooksBonus">Two rooks bonus value.</param>
        /// <param name="pawnValue">Pawn value.</param>
        /// <param name="knightValue">Knight value.</param>
        /// <param name="bishopValue">Bishop value.</param>
        /// <param name="rookValue">Rook value.</param>
        /// <param name="queenValue">Queen value.</param>
        /// <param name="useBetterEvaluator">Use Better Evaluation function value.</param>
        private void SetValues(int depth, int checkmateBonus, int checkBonus, int castleBonus, int mobilityMultiplier,
            int pieceMultiplier, int attackMultiplier, int depthMultiplier, int twoBishopsBonus, int twoRooksBonus,
            int pawnValue, int knightValue, int bishopValue, int rookValue, int queenValue, bool useBetterEvaluator)
        {
            // Set all values
            _depth.SliderValue = depth;
            _checkmateBonus.SliderValue = checkmateBonus;
            _checkBonus.SliderValue = checkBonus;
            _castleBonus.SliderValue = castleBonus;
            _mobilityMultiplier.SliderValue = mobilityMultiplier;
            _pieceMultiplier.SliderValue = pieceMultiplier;
            _attackMultiplier.SliderValue = attackMultiplier;
            _depthMultiplier.SliderValue = depthMultiplier;
            _twoBishopsBonus.SliderValue = twoBishopsBonus;
            _twoRooksBonus.SliderValue = twoRooksBonus;

            _pawnValue.SliderValue = pawnValue;
            _knightValue.SliderValue = knightValue;
            _bishopValue.SliderValue = bishopValue;
            _rookValue.SliderValue = rookValue;
            _queenValue.SliderValue = queenValue;

            _useBetterEvaluator.IsChecked = useBetterEvaluator;
        }
        
        /// <summary>
        /// Initialises components
        /// </summary>
        private void InitializeComponent()
        {
            // Load xaml
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Restore the default values of the AISettings.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void RestoreDefaults_OnClick(object? sender, RoutedEventArgs e)
        {
            // Set values TODO: Get rid of magic values
            SetValues(4, 100000, 50, 25, 1, 1, 1,
                100, 20, 50, 100, 300, 320, 500,
                900, false);
        }
        
        /// <summary>
        /// Called when the confirm button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            // Set static AI Settings values to the corresponding slider values
            Depth = _depth.SliderValue;
            CheckmateBonus = _checkmateBonus.SliderValue;
            CheckBonus = _checkBonus.SliderValue;
            CastleBonus = _castleBonus.SliderValue;
            MobilityMultiplier = _mobilityMultiplier.SliderValue;
            PieceMultiplier = _pieceMultiplier.SliderValue;
            AttackMultiplier = _attackMultiplier.SliderValue;
            DepthMultiplier = _depthMultiplier.SliderValue;
            TwoBishopsBonus = _twoBishopsBonus.SliderValue;
            TwoRooksBonus = _twoRooksBonus.SliderValue;

            PawnValue = _pawnValue.SliderValue;
            KnightValue = _knightValue.SliderValue;
            BishopValue = _bishopValue.SliderValue;
            RookValue = _rookValue.SliderValue;
            QueenValue = _queenValue.SliderValue;
            
            // TODO: Save to JSON
            // Close the window
            Close();
        }

        /// <summary>
        /// Called when the checkbox for using the better evaluator is checked
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void UseBetterEvaluator_OnChecked(object? sender, RoutedEventArgs e)
        {
            UseBetterEvaluator = true;
        }

        /// <summary>
        /// Called when the checkbox for using the better evaluator is unchecked
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void UseBetterEvaluator_OnUnchecked(object? sender, RoutedEventArgs e)
        {
            UseBetterEvaluator = false;
        }
    }
}