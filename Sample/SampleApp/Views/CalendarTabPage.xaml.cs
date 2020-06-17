﻿using System;
using SampleApp.ViewModels;
using Xamarin.Forms;

namespace SampleApp.Views
{
    public partial class CalendarTabPage : ContentPage
    {
        private readonly CalendarViewModel _viewModel;

        public CalendarTabPage()
        {
            BindingContext = _viewModel = new CalendarViewModel();
            InitializeComponent();
            _calendar1.ClickedDay += DayClicked;
        }

        private void DayClicked(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(sender is Global.CalendarView.Controls.DayCell cell)) return;
            if (cell.Date < DateTime.Today || cell.OutOfMonth) return;

            if (_viewModel.EndRange == _viewModel.BeginRange
                && _viewModel.EndRange != default
                && _viewModel.BeginRange != default)
            {
                if (cell.Date >= _viewModel.BeginRange)
                    _viewModel.EndRange = cell.Date;
                else
                    _viewModel.BeginRange = cell.Date;
            }
            else
            {
                _viewModel.BeginRange = cell.Date;
                _viewModel.EndRange = cell.Date;
            }
        }
    }
}