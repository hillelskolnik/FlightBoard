

// src/utils/flightUtils.ts
import { FlightStatus } from '../types/Flight';

export function getFlightStatus(departureTime: string): FlightStatus {
  const now = new Date();
  const departure = new Date(departureTime);
  const diff = (departure.getTime() - now.getTime()) / 60000; // difference in minutes

  if (diff > 30) return 'Scheduled';
  if (diff > 10) return 'Boarding';
  if (diff >= -60) return 'Departed';
  if (diff < -60) return 'Landed';
  if (diff < -15) return 'Delayed';
  return 'Scheduled';
}

export function getStatusColor(status: FlightStatus): string {
  switch (status) {
    case 'Scheduled':
      return '#4CAF50'; // Green
    case 'Boarding':
      return '#FF9800'; // Orange
    case 'Departed':
      return '#2196F3'; // Blue
    case 'Landed':
      return '#9E9E9E'; // Grey
    case 'Delayed':
      return '#F44336'; // Red
    default:
      return '#000000';
  }
}
