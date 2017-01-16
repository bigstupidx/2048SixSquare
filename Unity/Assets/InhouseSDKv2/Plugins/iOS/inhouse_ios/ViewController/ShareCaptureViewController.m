//
//  ShareCaptureViewController.m
//  Unity-iPhone
//
//  Created by Chau Giang on 10/18/16.
//
//

#import "ShareCaptureViewController.h"

@interface ShareCaptureViewController ()


@property (weak, nonatomic) IBOutlet UIView *node1;
@property (weak, nonatomic) IBOutlet UIView *node2;
@property (weak, nonatomic) IBOutlet UIView *node3;
@property (weak, nonatomic) IBOutlet UIView *node4;
@property (weak, nonatomic) IBOutlet UIView *node5;


@property (weak, nonatomic) IBOutlet UILabel *score1;
@property (weak, nonatomic) IBOutlet UILabel *score2;
@property (weak, nonatomic) IBOutlet UILabel *score3;
@property (weak, nonatomic) IBOutlet UILabel *score4;
@property (weak, nonatomic) IBOutlet UILabel *score5;

@property (weak, nonatomic) IBOutlet UILabel *mode1;
@property (weak, nonatomic) IBOutlet UILabel *mode2;
@property (weak, nonatomic) IBOutlet UILabel *mode3;
@property (weak, nonatomic) IBOutlet UILabel *mode4;
@property (weak, nonatomic) IBOutlet UILabel *mode5;


@property (nonatomic, strong) NSArray *array;
@property (nonatomic, strong) NSArray *arrayScore;
@property (nonatomic, strong) NSArray *arrayMode;



@end

@implementation ShareCaptureViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.array = @[_node1,_node2,_node3,_node4,_node5];
    self.arrayScore = @[_score1,_score2,_score3,_score4,_score5];
    self.arrayMode = @[_mode1,_mode2,_mode3,_mode4,_mode5];
   
    // Do any additional setup after loading the view from its nib.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (UIImage*) imageWithScore:(int)score type:(int)type mode:(NSString*)mode {
    [self view];
    ((UILabel *)[self.arrayScore objectAtIndex:type]).text = [NSString stringWithFormat:@"%d", score];
    ((UILabel *)[self.arrayMode objectAtIndex:type]).text = mode;
    
    UIImage* image = [self imageWithView:[self.array objectAtIndex:type]];
    return image;
}

- (UIImage *) imageWithView:(UIView *)view
{
    UIGraphicsBeginImageContextWithOptions(view.bounds.size, view.opaque, 0.0);
    [view.layer renderInContext:UIGraphicsGetCurrentContext()];
    
    UIImage * img = UIGraphicsGetImageFromCurrentImageContext();
    
    UIGraphicsEndImageContext();
    
    return img;
}
@end
