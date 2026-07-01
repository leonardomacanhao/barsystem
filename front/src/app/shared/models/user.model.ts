export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  groupId: string;
  groupName: string;
  tenantId?: string;
  tenantName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  groupName: string;
  groupDescription?: string;
  groupContactEmail?: string;
  groupContactPhone?: string;
  firstBarName: string;
  firstBarCnpj: string;
  adminName: string;
  adminEmail: string;
  adminPassword: string;
}

export interface LoginResponse {
  token: string;
  userId: string;
  groupId: string;
  groupName: string;
  tenantId?: string;
  tenantName?: string;
  name: string;
  email: string;
  role: string;
  expiresAt: string;
}
