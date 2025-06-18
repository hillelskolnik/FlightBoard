

import React from 'react';
import { Flight } from '../types/Flight';
import { getFlightStatus, getStatusColor } from '../utils/flightUtils';
import './FlightTable.css';

interface FlightTableProps {
  flights: Flight[];
  onDelete: (id: number) => void;
}

export const FlightTable: React.FC<FlightTableProps> = ({ flights, onDelete }) => {
  const formatDateTime = (dateTimeString: string) => {
    const date = new Date(dateTimeString);
    return date.toLocaleString();
  };

  return (
    <div className="flight-table-container">
      <table className="flight-table">
        <thead>
          <tr>
            <th>Flight Number</th>
            <th>Destination</th>
            <th>Departure Time</th>
            <th>Gate</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {flights.length === 0 ? (
            <tr>
              <td colSpan={6} className="no-flights">No flights available</td>
            </tr>
          ) : (
            flights.map((flight) => {
              const status = getFlightStatus(flight.departureTime);
              const statusColor = getStatusColor(status);
              
              return (
                <tr key={flight.id}>
                  <td>{flight.flightNumber}</td>
                  <td>{flight.destination}</td>
                  <td>{formatDateTime(flight.departureTime)}</td>
                  <td>{flight.gate}</td>
                  <td>
                    <span 
                      className="status-badge" 
                      style={{ backgroundColor: statusColor }}
                    >
                      {status}
                    </span>
                  </td>
                  <td>
                    <button 
                      onClick={() => onDelete(flight.id)}
                      className="delete-button"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              );
            })
          )}
        </tbody>
      </table>
    </div>
  );
};
