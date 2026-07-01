export interface Table {
  id: string;
  number: number;
  capacity: number;
  status: 'Free' | 'Occupied' | 'Reserved';
  location?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTableRequest {
  number: number;
  capacity: number;
  location?: string;
}

export interface UpdateTableRequest {
  number: number;
  capacity: number;
  location?: string;
}

export interface UpdateTableStatusRequest {
  status: 'Free' | 'Occupied' | 'Reserved';
}
