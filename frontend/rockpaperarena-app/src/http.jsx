

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5173/Api';

// Constants for game moves
export const MOVES = {
  ROCK: 1,
  PAPER: 2,
  SCISSORS: 3,
};

// Constants for HTTP status codes
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  INTERNAL_SERVER_ERROR: 500,
  SERVICE_UNAVAILABLE: 503,
};



// Helper function to handle API responses with detailed status handling
const handleResponse = async (response) => {
  if (!response.ok) {
    let errorMessage = 'An error occurred';
    
    try {
      const errorData = await response.json();
      errorMessage = errorData.error || errorMessage;
    } catch {
      // If JSON parsing fails, use status-based messages
      switch (response.status) {
        case HTTP_STATUS.BAD_REQUEST:
          errorMessage = 'Invalid request. Please check your input.';
          break;
        case HTTP_STATUS.UNAUTHORIZED:
          errorMessage = 'Unauthorized access.';
          break;
        case HTTP_STATUS.FORBIDDEN:
          errorMessage = 'Access forbidden.';
          break;
        case HTTP_STATUS.NOT_FOUND:
          errorMessage = 'Resource not found.';
          break;
        case HTTP_STATUS.CONFLICT:
          errorMessage = 'Conflict with current state.';
          break;
        case HTTP_STATUS.INTERNAL_SERVER_ERROR:
          errorMessage = 'Server error. Please try again later.';
          break;
        case HTTP_STATUS.SERVICE_UNAVAILABLE:
          errorMessage = 'Service temporarily unavailable.';
          break;
        default:
          errorMessage = `HTTP error! status: ${response.status}`;
      }
    }
    
    const error = new Error(errorMessage);
    error.status = response.status;
    throw error;
  }
  
  return response.json();
};


// Tournament endpoints
export const startTournament = async (name, players) => {
  const response = await fetch(`${API_BASE_URL}/tournament/start`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ name, players }),
  });
  return handleResponse(response);
};

export const getTournamentStatus = async () => {
  const response = await fetch(`${API_BASE_URL}/tournament/status`, {
    method: 'GET',
  });
  return handleResponse(response);
};

export const playMove = async (move) => {
  const response = await fetch(`${API_BASE_URL}/tournament/play`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(move),
  });
  return handleResponse(response);
};

export const advanceTournament = async () => {
  const response = await fetch(`${API_BASE_URL}/tournament/advance`, {
    method: 'POST',
  });
  return handleResponse(response);
};

export const getFinalResult = async () => {
  const response = await fetch(`${API_BASE_URL}/tournament/final`, {
    method: 'GET',
  });
  return handleResponse(response);
};




// Participant endpoints
export const addPlayer = async (name) => {
  const response = await fetch(`${API_BASE_URL}/player`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(name),
  });
  return handleResponse(response);
};





export default {
  startTournament,
  getTournamentStatus,
  playMove,
  advanceTournament,
  getFinalResult,
  addPlayer,
  MOVES,
};