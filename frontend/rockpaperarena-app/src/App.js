import './App.css';
import { useEffect, useState, Fragment} from 'react';
import { startTournament, getTournamentStatus, playMove, getFinalResult, MOVES } from './http';
import { Button, Container, Typography, Dialog, DialogActions, 
        DialogContent, DialogContentText, DialogTitle, TextField, Box, 
        Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';

function App() {

  const [scoreboard, setScoreboard] = useState([]);
  const [tournamentActive, setTournamentActive] = useState(false); // Track if tournament is active
  const [tournamentInfo, setTournamentInfo] = useState(null); // Store tournament info

  const [open, setOpen] = useState(false);
  const [dialogStep, setDialogStep] = useState(1);
  const [playerName, setPlayerName] = useState('');
  const [numberOfPlayers, setNumberOfPlayers] = useState('');

  const handleClickOpen = () => {
    setDialogStep(1);
    setPlayerName('');
    setNumberOfPlayers('');
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setDialogStep(1);
  };

  const handleNameSubmit = (event) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const formJson = Object.fromEntries(formData.entries());
    const name = formJson.Name;
    console.log('Player name:', name);
    setPlayerName(name);
    setDialogStep(2);
  };

  const handlePlayersSubmit = async (event) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const formJson = Object.fromEntries(formData.entries());
    const players = parseInt(formJson.players);
    console.log('Number of players:', players);
    setNumberOfPlayers(players);
    
    try {
      // Start the tournament
      const response = await startTournament(playerName, players);
      console.log('Tournament started:', response);
      
      // Set tournament as active and store info
      setTournamentActive(true);
      setTournamentInfo(response);
      
      handleClose();
    } catch (error) {
      console.error('Error starting tournament:', error);
      alert(`Error starting tournament: ${error.message}`);
    }
  };

  const handleBack = () => {
    setDialogStep(1);
  };

  const handlePlayMove = async (moveValue) => {
    try {
      const response = await playMove(moveValue);
      console.log('Move played:', response);
      // Update your state based on response
      // setScoreboard(response.scoreboard);
    } catch (error) {
      console.error('Error playing move:', error);
      alert(`Error playing move: ${error.message}`);
    }
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ 
        display: 'flex', 
        flexDirection: 'column', 
        alignItems: 'center',
        minHeight: '100vh',
        py: 4,
        gap: 2
      }}>
        <Typography variant="h1" gutterBottom>Rock Paper Arena</Typography>

        {!tournamentActive ? (
          // Show "Play Game" button when no tournament is active
          <Fragment>
            <Button variant="outlined" size='large' onClick={handleClickOpen}>
              Play Game
            </Button>
            <Dialog open={open} onClose={handleClose}>
              {dialogStep === 1 ? (
                // Step 1: Name input
                <>
                  <DialogTitle>Welcome</DialogTitle>
                  <DialogContent>
                    <DialogContentText>
                      To play, please enter your name here.
                    </DialogContentText>
                    <form onSubmit={handleNameSubmit} id="name-form">
                      <TextField
                        autoFocus
                        required
                        margin="dense"
                        id="name"
                        name="Name"
                        label="Name"
                        type="text"
                        fullWidth
                        variant="standard"
                        value={playerName}
                        onChange={(e) => setPlayerName(e.target.value)}
                      />
                    </form>
                  </DialogContent>
                  <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button type="submit" form="name-form">
                      Next
                    </Button>
                  </DialogActions>
                </>
              ) : (
                // Step 2: Number of players input
                <>
                  <DialogTitle>Tournament Setup</DialogTitle>
                  <DialogContent>
                    <DialogContentText>
                      Hi {playerName}! How many players will participate? (Must be a power of 2: 2, 4, 8, 16, etc.)
                    </DialogContentText>
                    <form onSubmit={handlePlayersSubmit} id="players-form">
                      <TextField
                        autoFocus
                        required
                        margin="dense"
                        id="players"
                        name="players"
                        label="Number of Players"
                        type="number"
                        fullWidth
                        variant="standard"
                        value={numberOfPlayers}
                        onChange={(e) => setNumberOfPlayers(e.target.value)}
                        inputProps={{ min: 2, step: 1 }}
                      />
                    </form>
                  </DialogContent>
                  <DialogActions>
                    <Button onClick={handleBack}>Back</Button>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button type="submit" form="players-form">
                      Start Tournament
                    </Button>
                  </DialogActions>
                </>
              )}
            </Dialog>
          </Fragment>
        ) : (
          // Show game buttons when tournament is active
          <>
            {tournamentInfo && (
              <Typography variant="h6" gutterBottom>
                Welcome {tournamentInfo.playerName}! Round {tournamentInfo.currentRound} of {tournamentInfo.totalRounds}
              </Typography>
            )}
            
            <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
              <Button 
                variant="outlined"
                size="large"
                onClick={() => handlePlayMove(MOVES.ROCK)}
              >
                Rock
              </Button>

              <Button 
                variant="outlined"
                size="large"
                onClick={() => handlePlayMove(MOVES.PAPER)}
              >
                Paper
              </Button>

              <Button 
                variant="outlined"
                size="large"
                onClick={() => handlePlayMove(MOVES.SCISSORS)}
              >
                Scissors
              </Button>
            </Box>
          </>
        )}
      </Box>
    </Container>
  );
}

export default App;