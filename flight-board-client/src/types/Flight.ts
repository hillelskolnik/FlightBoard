
export interface Flight {
    id: number;
    flightNumber: string;
    destination: string;
    departureTime: string;
    gate: string;
  }
  
  export interface CreateFlight {
    flightNumber: string;
    destination: string;
    departureTime: string;
    gate: string;
  }
  
  export type FlightStatus = 'Scheduled' | 'Boarding' | 'Departed' | 'Landed' | 'Delayed';
