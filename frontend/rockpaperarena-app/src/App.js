import './App.css';
import { useEffect, useState, Fragment} from 'react';
import { startTournament, getTournamentStatus, playMove, getFinalResult, MOVES, advanceTournament } from './http';
import { Button, Container, Typography, Dialog, DialogActions, 
        DialogContent, DialogContentText, DialogTitle, TextField, Box, 
        Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Stack } from '@mui/material';
import LandscapeIcon from '@mui/icons-material/Landscape';
import ContentCutIcon from '@mui/icons-material/ContentCut';
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined';



function App() {

  const [scoreboard, setScoreboard] = useState([]);
  const [tournamentActive, setTournamentActive] = useState(false);
  const [tournamentInfo, setTournamentInfo] = useState(null);

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
      const response = await startTournament(playerName, players);
      console.log('Tournament started:', response);
      
      var scoreboard = response.scoreboard || [];

      setTournamentActive(true);
      setTournamentInfo(response);
      
      // Extract scoreboard from the nested structure
      if (response.scoreboard && response.scoreboard.scores) {
        // Transform the scores object into an array for the table
        const scoresArray = Object.entries(response.scoreboard.scores).map(([playerId, playerData]) => ({
          playerId: parseInt(playerId),
          playerName: playerData.name || `Player ${playerId}`,
          wins: playerData.wins || 0,
          losses: playerData.losses || 0,
          draws: playerData.draws || 0,
          points: playerData.points || 0
        }));
      
        // Sort by points descending
        scoresArray.sort((a, b) => b.points - a.points);
        
        setScoreboard(scoresArray);
    }
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
    
    // Update tournament info with the response data
    setTournamentInfo({
      playerName: response.player,
      opponent: response.opponent,
      currentRound: response.currentRound,
      totalRounds: tournamentInfo.totalRounds, // Keep from initial state
      player1Wins: response.player1Wins,
      player2Wins: response.player2Wins,
      subRound: response.subRound,
      draws: response.draws,
      isComplete: response.isComplete
    });

    // Update scoreboard from response if it exists
    if (response.scoreboard && response.scoreboard.scores) {
      const scoresArray = Object.entries(response.scoreboard.scores).map(([playerId, playerData]) => ({
        playerId: parseInt(playerId),
        playerName: playerData.name || `Player ${playerId}`,
        wins: playerData.wins || 0,
        losses: playerData.losses || 0,
        draws: playerData.draws || 0,
        points: playerData.points || 0
      }));
      
      // Sort by points descending
      scoresArray.sort((a, b) => b.points - a.points);
      setScoreboard(scoresArray);
    }
    
    // Update tournament info if provided
    if (response.tournamentInfo) {
      setTournamentInfo(response.tournamentInfo);
    }
  } catch (error) {
    console.error('Error playing move:', error);
    alert(`Error playing move: ${error.message}`);
  }
};

const handleClickAdvanceGame = async () => {
  try {
    const response = await advanceTournament();
    console.log('Tournament advanced:', response);
        
    console.log('isComplete = ', response.isComplete);
    //console.log('isComplete type:', typeof response.isComplete); // Add this to check the type

    const isComplete = response.isComplete === true || response.isComplete === 'true';

    if (isComplete) {
      console.log('Tournament is complete!');
      const finalResult = await getFinalResult();
      console.log('Final result:', finalResult);    

      const isTie = finalResult.isTie === true || finalResult.isTie === 'true';
      if(isTie) {
        alert(`${finalResult.message}  ${finalResult.winners}`);
      } else {

      alert(`Tournament Complete! Winner: ${finalResult.winners}`);
      setTournamentActive(false);
      setTournamentInfo(null);
      setScoreboard([]);
      }

    } else {
      // Update tournament info and scoreboard if not complete
      setTournamentInfo(response);
      if (response.scoreboard && response.scoreboard.scores) {
        const scoresArray = Object.entries(response.scoreboard.scores).map(([playerId, playerData]) => ({
          playerId: parseInt(playerId),
          playerName: playerData.name || `Player ${playerId}`,
          totalRounds: tournamentInfo.totalRounds, // Keep from initial state
          wins: playerData.wins || 0,
          losses: playerData.losses || 0,
          draws: playerData.draws || 0,
          points: playerData.points || 0
        }));
        
        // Sort by points descending
        scoresArray.sort((a, b) => b.points - a.points);
        setScoreboard(scoresArray);
      }
    }
  } catch (error) {
    console.error('Error advancing tournament:', error);
    alert(`Error advancing tournament: ${error.message}`);
  }
}





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
          <Fragment>
            <Button variant="outlined" size='large' onClick={handleClickOpen}>
              Play Game
            </Button>
            <Dialog open={open} onClose={handleClose}>
              {dialogStep === 1 ? (
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
          <>
            {tournamentInfo && (
              <Typography variant="h6" gutterBottom align="center">
                Round {tournamentInfo.currentRound}: {tournamentInfo.playerName} vs {tournamentInfo.opponent}. 
                <br />Sub round {tournamentInfo.subRound || 1} of 3
                <br />Score: {tournamentInfo.player1Wins || 0} - {tournamentInfo.player2Wins || 0}
                <br />Draws: {tournamentInfo.draws || 0}
              </Typography>
            )}
            
            <Stack direction="row" spacing={2}>
              <Button 
                variant="text"
                size="large"
                onClick={() => handlePlayMove(MOVES.ROCK)}
                sx={{ 
                  minWidth: 200, 
                  minHeight: 200,
                  fontSize: '3rem'
                }}
              >
                <LandscapeIcon sx={{ fontSize: 100 }}/>
              </Button>

              <Button 
                variant="text"
                size="large"
                onClick={() => handlePlayMove(MOVES.PAPER)}
                sx={{ 
                  minWidth: 200, 
                  minHeight: 200,
                  fontSize: '3rem'
                }}
              >
                <DescriptionOutlinedIcon sx={{ fontSize: 100 }}/>
              </Button>

              <Button 
                variant="text"
                size="large"
                onClick={() => handlePlayMove(MOVES.SCISSORS)}
                sx={{ 
                  minWidth: 200, 
                  minHeight: 200,
                  fontSize: '3rem'
                }}
              >
                <ContentCutIcon sx={{ fontSize: 100 }}/>
              </Button>
            </Stack>

            {/* Scoreboard Table */}
            {scoreboard && scoreboard.length > 0 && (
              <Box sx={{ mt: 4, width: '100%', maxWidth: 800 }}>
                <Typography variant="h5" gutterBottom>
                  Scoreboard
                </Typography>
                <TableContainer component={Paper}>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell><strong>Rank</strong></TableCell>
                        <TableCell><strong>ID</strong></TableCell>
                        <TableCell><strong>Name</strong></TableCell>
                        <TableCell align="right"><strong>Wins</strong></TableCell>
                        <TableCell align="right"><strong>Losses</strong></TableCell>
                        <TableCell align="right"><strong>Draws</strong></TableCell>
                        <TableCell align="right"><strong>Points</strong></TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {scoreboard.map((entry, index) => (
                        <TableRow 
                          key={entry.playerId || index}
                          sx={{ 
                            '&:last-child td, &:last-child th': { border: 0 },
                            backgroundColor: entry.playerName === playerName ? 'action.selected' : 'inherit'
                          }}
                        >
                          <TableCell>{index + 1}</TableCell>
                          <TableCell>{entry.playerId}</TableCell>
                          <TableCell>
                            {entry.playerName}
                            {entry.playerName === playerName && ' (You)'}
                          </TableCell>
                          <TableCell align="right">{entry.wins || 0}</TableCell>
                          <TableCell align="right">{entry.losses || 0}</TableCell>
                          <TableCell align="right">{entry.draws || 0}</TableCell>
                          <TableCell align="right">{entry.points || 0}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Box>              
            )}

            <Button variant="outlined" size='large' onClick={handleClickAdvanceGame}>
              Next Round
            </Button>
          </>
        )}
      </Box>
    </Container>
  );
}

export default App;