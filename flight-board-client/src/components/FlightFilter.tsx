
import React, { useState } from 'react';
import './FlightFilter.css';

interface FlightFilterProps {
  onSearch: (status?: string, destination?: string) => void;
  onClear: () => void;
}

export const FlightFilter: React.FC<FlightFilterProps> = ({ onSearch, onClear }) => {
  const [status, setStatus] = useState('');
  const [destination, setDestination] = useState('');

  const handleSearch = () => {
    onSearch(status || undefined, destination || undefined);
  };

  const handleClear = () => {
    setStatus('');
    setDestination('');
    onClear();
  };

  return (
    <div className="flight-filter">
      <h3>Filter Flights</h3>
      <div className="filter-group">
        <label htmlFor="statusFilter">Status:</label>
        <select
          id="statusFilter"
          value={status}
          onChange={(e) => setStatus(e.target.value)}
        >
          <option value="">All</option>
          <option value="Scheduled">Scheduled</option>
          <option value="Boarding">Boarding</option>
          <option value="Departed">Departed</option>
          <option value="Landed">Landed</option>
          <option value="Delayed">Delayed</option>
        </select>
      </div>

      <div className="filter-group">
        <label htmlFor="destinationFilter">Destination:</label>
        <input
          type="text"
          id="destinationFilter"
          value={destination}
          onChange={(e) => setDestination(e.target.value)}
          placeholder="Enter destination"
        />
      </div>

      <div className="filter-buttons">
        <button onClick={handleSearch} className="search-button">Search</button>
        <button onClick={handleClear} className="clear-button">Clear Filters</button>
      </div>
    </div>
  );
};
