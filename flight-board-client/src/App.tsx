

import React, { useState, useEffect, useCallback } from 'react';
import { Flight, CreateFlight } from './types/Flight';
import { flightService } from './services/flightService';
import { FlightTable } from './components/FlightTable';
import { FlightForm } from './components/FlightForm';
import { FlightFilter } from './components/FlightFilter';
import './App.css';

function App() {
  const [flights, setFlights] = useState<Flight[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<{ status?: string; destination?: string }>({});

  // Fetch flights function
  const fetchFlights = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await flightService.getFlights(filters.status, filters.destination);
      setFlights(data);
    } catch (err) {
      setError('Failed to fetch flights. Please try again.');
      console.error('Error fetching flights:', err);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  // Initial load and auto-refresh every 2 minutes
  useEffect(() => {
    fetchFlights();
    
    const interval = setInterval(() => {
      fetchFlights();
    }, 2 * 60 * 1000); // 2 minutes

    return () => clearInterval(interval);
  }, [fetchFlights]);

  // Add flight handler
  const handleAddFlight = async (flightData: CreateFlight) => {
    try {
      setError(null);
      await flightService.addFlight(flightData);
      fetchFlights(); // Refresh the list
    } catch (err: any) {
      if (err.response?.data) {
        setError(err.response.data);
      } else {
        setError('Failed to add flight. Please try again.');
      }
      console.error('Error adding flight:', err);
    }
  };

  // Delete flight handler
  const handleDeleteFlight = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this flight?')) {
      try {
        setError(null);
        await flightService.deleteFlight(id);
        fetchFlights(); // Refresh the list
      } catch (err) {
        setError('Failed to delete flight. Please try again.');
        console.error('Error deleting flight:', err);
      }
    }
  };

  // Search handler
  const handleSearch = (status?: string, destination?: string) => {
    setFilters({ status, destination });
  };

  // Clear filters handler
  const handleClearFilters = () => {
    setFilters({});
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>Flight Board Management System</h1>
        <p>Real-time flight information updated every 2 minutes</p>
      </header>

      {error && <div className="error">{error}</div>}

      <FlightForm onSubmit={handleAddFlight} />
      
      <FlightFilter 
        onSearch={handleSearch} 
        onClear={handleClearFilters} 
      />

      {loading ? (
        <div className="loading">Loading flights...</div>
      ) : (
        <FlightTable flights={flights} onDelete={handleDeleteFlight} />
      )}
    </div>
  );
}

export default App;
